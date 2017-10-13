using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WhackAMole.UWPClient.Controls;
using WhackAMole.UWPClient.Services;
using WhackAMole.UWPClient.ViewModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WhackAMole.UWPClient.Models
{
    class WhackSpace : INotifyPropertyChanged
    {
        private const int UPDATE_INTERVAL = 100;
        private const int TICK_INTERVAL = 33;
        private const int MOLE_SPEED = 5;
        private readonly double MOLE_SIZE = 75;
        private readonly TimeSpan _expired = TimeSpan.FromMinutes(2);
        private DateTime _lastKill;
        
        SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);
        private readonly IMovementEngine _movementEngine;
        private readonly MoleService _moleService;
        private readonly AdminService _adminService;

        ThreadPoolTimer _timer;
        int _countdown;

        private Canvas _molepen;

        private Dictionary<string, MoleControl> _molemap = new Dictionary<string, MoleControl>();
        private List<string> _removedList = new List<string>();

        public SynchronizedObservableCollection<MoleViewModel> Moles { get; private set; } = new SynchronizedObservableCollection<MoleViewModel>();
        public ObservableCollection<KubeNode> Nodes { get; private set; } = new ObservableCollection<KubeNode>();

        private Task _updateTask = null;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler Inactive;


        private double _width;
        public double Width
        {
            get { return _width; }
            set {
                if (_width == value)
                    return;
                if (_width != 0)
                    UpdateSize();
                Debug.WriteLine("space width " + value);
                _width = value;
            }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set {
                if (_height == value)
                    return;
                if (_height != 0)
                    UpdateSize();
                Debug.WriteLine("space height " + value);
                _height = value;
            }
        }

        private void UpdateSize()
        {
            _movementEngine.Width = Width;
            _movementEngine.Height = Height;
        }

        private int _whackCount;
        public int WhackCount
        {
            get { return _whackCount; }
            set
            {
                if (_whackCount == value)
                    return;
                _whackCount = value;
                RaisePropertyChanged(nameof(WhackCount));
            }
        }

     
        public WhackSpace( double width, double height)
        {
           
            Width = width;
            Height = height;

            _movementEngine = new BaseMovementEngine(Width, Height,MOLE_SIZE);
            var moleEndpoint = ApplicationData.Current.LocalSettings.Values["moleServiceEndpoint"] as string;
            var adminEndpoint = ApplicationData.Current.LocalSettings.Values["adminServiceEndpoint"] as string;
          
            MoleService.Create(moleEndpoint);
           
            AdminService.Create(adminEndpoint);
            _moleService = MoleService.Instance;
            _adminService = AdminService.Instance;
        }

        public async Task SetupAsync(Canvas molepen)
        {
            _molepen = molepen;
            Reset();
           
            await UpdateNodeListAsync();

            await UpdatePodListAsync();
        }

        public void Start()
        {
            _timer = ThreadPoolTimer.CreatePeriodicTimer(_timer_Tick, TimeSpan.FromMilliseconds(TICK_INTERVAL));
            _lastKill = DateTime.Now;
        }



        private  void _timer_Tick(object source)
        {
            _countdown += TICK_INTERVAL;
            foreach (var mole in Moles)
            {
                var newMovement = _movementEngine.UpdatePosition(mole.CurrentPosition, mole.Vector);
                mole.CurrentPosition = newMovement.Item1;
                mole.Vector = newMovement.Item2;
                AlignDisplay(mole);
            }
          
                

            if (_countdown > 500)
            {
                _countdown = 0;
                if (_updateTask == null  || _updateTask.IsFaulted || _updateTask.IsCompleted)
                {
                    _updateTask = UpdatePodListAsync();
                    _countdown = 0;
                }
                if (DateTime.Now - _lastKill > _expired)
                    Stop();
            }
        }

        private void Stop()
        {
            _timer.Cancel();
            _timer = null;
            Inactive?.Invoke(this, null);
        }

        private async Task<List<KubeNode>> GetNodesAsync()
        {
            Debug.WriteLine("Geting Nodes");
            var nodes = await _adminService.GetNodesAsync();
            return nodes;
        }

        private void Reset()
        {
            Moles.Clear();
            Nodes.Clear();
            _removedList.Clear();
            _molepen.Children.Clear();
            WhackCount = 0;
        }

        private async Task UpdateNodeListAsync()
        {
            Debug.WriteLine("Updating Node List");
            var nodes = await GetNodesAsync();
            foreach (var node in nodes)
                Nodes.Add(node);
        }


        // not happy with this... seems there should be more elegant way but it works for now
        private async Task UpdatePodListAsync()
        {
            await RefreshMoleState();
            var newList = await _adminService.GetPodsAsync();

            // This is to account for the fact that the management API is deployed as part
            // of the moleservice in Kubernetes.  As pods are killed, the management API may
            // become unavailable and we do not get a pod list.
            // Best fix is to break out the management API into its own container and 
            // Kubernetes service.  Add it to the todo list.... sigh.
            if (newList == null || newList.Count == 0) return;

            var currentList = from m in Moles select new KubePod { Name = m.MoleName };
           

            Debug.WriteLine("New Pods");
            var newPods = newList.Except(currentList,new KubePodComparer());
            foreach (var p in newPods)
            {
                if (!_removedList.Contains(p.Name))
                {
                    await CreateMoleAsync(p);
                    Debug.WriteLine($"\t{p.Name}");
                }
            }

            Debug.WriteLine("Deleted Pods");
            var deletedPods = currentList.Except(newList, new KubePodComparer());
            foreach (var p in deletedPods)
            {
                await RemoveMoleAsync(p.Name);
                Debug.WriteLine($"\t{p.Name}");
            }

            if (deletedPods.Count() == 0 && newPods.Count() == 0)
                _removedList.Clear();

            Debug.WriteLine("Updating Pod List: ");
            foreach (var p in newList)
            {
                var mole = (from m in Moles where m.MoleName == p.Name select m).SingleOrDefault();
                if (mole != null)
                {
                    mole.Phase = p.Phase;
                    Debug.WriteLine($"\t{mole.MoleName} - {mole.Phase}");
                }
            }


        }

 
        private async Task RefreshMoleState()
        {
            foreach (var m in Moles)
            {
                await m.UpdateStateAsync();
            }
         
        }

        private async Task<MoleControl> CreateMoleAsync(KubePod pod)
        {
            try
            {
                MoleControl moleControl = null;
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    
                
                    moleControl = new MoleControl();
                    _molemap.Add(pod.Name, moleControl);
                    moleControl.MoleName = pod.Name;
                    moleControl.Height = moleControl.Width = MOLE_SIZE;
                    var init = _movementEngine.InitializeMole(MOLE_SPEED);
                    var vm = new MoleViewModel(init.Item1, init.Item2, _moleService, _adminService) { CurrentChar = "-", MoleName = pod.Name, Phase = pod.Phase };
                    moleControl.DataContext = vm;
                    Moles.Add(vm);
                    var property = nameof(vm.DisplayAlignment);
                    Binding binding = new Binding() { Path = new PropertyPath(property) };
                    moleControl.SetBinding(MoleControl.DisplayAlignmentProperty, binding);

                    moleControl.Init(_width, _height);
                    moleControl.MoleTapped += async (s, e) =>
                    {
                        Debug.WriteLine($"MOLE TAPPED {e.MoleName}");
                        await DeletePodAsync(e.Mole);
                        _lastKill = DateTime.Now;
                    };

                    _molepen.Children.Add(moleControl);
                });
           
                await _semaphore.WaitAsync();
                Debug.WriteLine($"+++Create mole {pod.Name}");
                return moleControl;
            }
            finally {
                _semaphore.Release();
            }
            
        }

        private void RemoveMoleFromPen(MoleControl mole)
        {
            if (mole != null)
            {
                _molepen.Children.Remove(mole);
                _molemap.Remove(mole.MoleName);
            }
        }

        private void RemoveMoleFromPen(string molename)
        {
            if (!_molemap.Keys.Contains(molename))
                return;

            var mole = _molemap[molename];
            RemoveMoleFromPen(mole);
        }

        private async Task<bool> DeletePodAsync(MoleViewModel mole)
        {
            var result = await mole.DeleteAsync();
            if (result)
            {
                await RemoveMoleAsync(mole);
                WhackCount++;
                 
            }
            Debug.WriteLine($"### Pod Deleted - {mole.MoleName}");
            return result;
        }

        private async Task RemoveMoleAsync(MoleViewModel remove)
        {
            try
            {
                await _semaphore.WaitAsync();
                _removedList.Add(remove.MoleName);
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Moles.Remove(remove);
                    RemoveMoleFromPen(remove.MoleName);
                    remove = null;
                });
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveMoleAsync(string molename)
        {
            var mole = (from m in Moles where m.MoleName == molename select m).SingleOrDefault();
            if (mole != null)
                await RemoveMoleAsync(mole);
        }

        private void AlignDisplay(MoleViewModel mole)
        {
            if (mole.Vector.X > 0)
                mole.DisplayAlignment = (mole.Vector.Y > 0) ? MoleAlignment.TopLeft : MoleAlignment.BottomLeft;
            else
                mole.DisplayAlignment = (mole.Vector.Y > 0) ? MoleAlignment.TopRight : MoleAlignment.BottomRight;
        }

        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}

﻿using ReactiveUI;
using System.Reactive.Linq;
using Zametek.Common.ProjectPlan;
using Zametek.Contract.ProjectPlan;

namespace Zametek.ViewModel.ProjectPlan
{
    public class ResourceTrackersViewModel
        : ViewModelBase, IResourceTrackersViewModel
    {
        #region Fields

        private readonly object m_Lock;
        private readonly ICoreViewModel m_CoreViewModel;
        private readonly IManagedResourceViewModel m_ManagedResourceViewModel;
        private readonly Dictionary<int, IResourceActivitySelectorViewModel> m_ResourceActivitySelectorLookup;

        private readonly IDisposable? m_DaysSub;

        #endregion

        #region Ctors

        public ResourceTrackersViewModel(
            ICoreViewModel coreViewModel,
            IManagedResourceViewModel managedResourceViewModel,
            int resourceId,
            IEnumerable<ResourceTrackerModel> trackers)
        {
            ArgumentNullException.ThrowIfNull(coreViewModel);
            ArgumentNullException.ThrowIfNull(managedResourceViewModel);
            m_Lock = new object();
            m_CoreViewModel = coreViewModel;
            m_ManagedResourceViewModel = managedResourceViewModel;
            ResourceId = resourceId;
            m_ResourceActivitySelectorLookup = [];

            foreach (ResourceTrackerModel tracker in trackers)
            {
                if (tracker.ResourceId == ResourceId)
                {
                    var selector = new ResourceActivitySelectorViewModel(m_CoreViewModel, tracker);
                    m_ResourceActivitySelectorLookup.TryAdd(tracker.Time, selector);
                }
            }

            m_DaysSub = this
                .WhenAnyValue(x => x.m_CoreViewModel.TrackerIndex)
                .ObserveOn(RxApp.TaskpoolScheduler) // TODO check this will work.
                .Subscribe(_ => RefreshDays());
        }

        #endregion

        #region Private Members

        private int TrackerIndex => m_CoreViewModel.TrackerIndex;

        private IResourceActivitySelectorViewModel GetResourceActivitySelector(int index)
        {
            lock (m_Lock)
            {

                int indexOffset = index + TrackerIndex;

                if (!m_ResourceActivitySelectorLookup.TryGetValue(indexOffset, out IResourceActivitySelectorViewModel? selector))
                {
                    // If the selector does not exist, but we are currently editing
                    // the managed resource, then create a new selector and add it
                    // to the lookup dictionary.
                    if (m_ManagedResourceViewModel.IsEditing)
                    {
                        selector = new ResourceActivitySelectorViewModel(
                            m_CoreViewModel,
                            new ResourceTrackerModel
                            {
                                Time = indexOffset,
                                ResourceId = ResourceId,
                            });
                        m_ResourceActivitySelectorLookup.Add(indexOffset, selector);
                    }
                    // Otherwise, just return the empty one. Since we only need to
                    // create a new selector during editing.
                    else
                    {
                        selector = ResourceActivitySelectorViewModel.Empty;
                    }
                }

                // TODO
                // TODO clean up empty selectors at compile time.
                return selector;
            }
        }

        private void RefreshDays()
        {
            this.RaisePropertyChanged(nameof(Day00));
            this.RaisePropertyChanged(nameof(Day01));
            this.RaisePropertyChanged(nameof(Day02));
            this.RaisePropertyChanged(nameof(Day03));
            this.RaisePropertyChanged(nameof(Day04));
            this.RaisePropertyChanged(nameof(Day05));
            this.RaisePropertyChanged(nameof(Day06));
            this.RaisePropertyChanged(nameof(Day07));
            this.RaisePropertyChanged(nameof(Day08));
            this.RaisePropertyChanged(nameof(Day09));
            this.RaisePropertyChanged(nameof(Day10));
            this.RaisePropertyChanged(nameof(Day11));
            this.RaisePropertyChanged(nameof(Day12));
            this.RaisePropertyChanged(nameof(Day13));
            this.RaisePropertyChanged(nameof(Day14));
            this.RaisePropertyChanged(nameof(Day15));
            this.RaisePropertyChanged(nameof(Day16));
            this.RaisePropertyChanged(nameof(Day17));
            this.RaisePropertyChanged(nameof(Day18));
            this.RaisePropertyChanged(nameof(Day19));
        }

        #endregion

        #region IResourceTrackerViewModel Members

        public List<ResourceTrackerModel> Trackers
        {
            get
            {
                return m_ResourceActivitySelectorLookup.Values
                    .Where(selector => selector.SelectedResourceActivityIds.Count > 0)
                    .OrderBy(selector => selector.Time)
                    .Select(selector =>
                    {
                        List<ResourceActivityTrackerModel> resourceActivityTrackers = selector.SelectedTargetResourceActivities
                            .Select(activity =>
                            {
                                return new ResourceActivityTrackerModel
                                {
                                    Time = selector.Time,
                                    ResourceId = selector.ResourceId,
                                    ActivityId = activity.Id,
                                    ActivityName = activity.Name,
                                    PercentageWorked = activity.PercentageWorked,
                                };
                            }).ToList();

                        return new ResourceTrackerModel
                        {
                            Time = selector.Time,
                            ResourceId = selector.ResourceId,
                            ActivityTrackers = resourceActivityTrackers,
                        };
                    }).ToList();
            }
        }

        public int ResourceId { get; }

        public IResourceActivitySelectorViewModel Day00
        {
            get => GetResourceActivitySelector(0);
        }

        public IResourceActivitySelectorViewModel Day01
        {
            get => GetResourceActivitySelector(1);
        }

        public IResourceActivitySelectorViewModel Day02
        {
            get => GetResourceActivitySelector(2);
        }

        public IResourceActivitySelectorViewModel Day03
        {
            get => GetResourceActivitySelector(3);
        }

        public IResourceActivitySelectorViewModel Day04
        {
            get => GetResourceActivitySelector(4);
        }

        public IResourceActivitySelectorViewModel Day05
        {
            get => GetResourceActivitySelector(5);
        }

        public IResourceActivitySelectorViewModel Day06
        {
            get => GetResourceActivitySelector(6);
        }

        public IResourceActivitySelectorViewModel Day07
        {
            get => GetResourceActivitySelector(7);
        }

        public IResourceActivitySelectorViewModel Day08
        {
            get => GetResourceActivitySelector(8);
        }

        public IResourceActivitySelectorViewModel Day09
        {
            get => GetResourceActivitySelector(9);
        }

        public IResourceActivitySelectorViewModel Day10
        {
            get => GetResourceActivitySelector(10);
        }

        public IResourceActivitySelectorViewModel Day11
        {
            get => GetResourceActivitySelector(11);
        }

        public IResourceActivitySelectorViewModel Day12
        {
            get => GetResourceActivitySelector(12);
        }

        public IResourceActivitySelectorViewModel Day13
        {
            get => GetResourceActivitySelector(13);
        }

        public IResourceActivitySelectorViewModel Day14
        {
            get => GetResourceActivitySelector(14);
        }

        public IResourceActivitySelectorViewModel Day15
        {
            get => GetResourceActivitySelector(15);
        }

        public IResourceActivitySelectorViewModel Day16
        {
            get => GetResourceActivitySelector(16);
        }

        public IResourceActivitySelectorViewModel Day17
        {
            get => GetResourceActivitySelector(17);
        }

        public IResourceActivitySelectorViewModel Day18
        {
            get => GetResourceActivitySelector(18);
        }

        public IResourceActivitySelectorViewModel Day19
        {
            get => GetResourceActivitySelector(19);
        }

        #endregion

        #region IDisposable Members

        private bool m_Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                m_DaysSub?.Dispose();
                foreach (IResourceActivitySelectorViewModel selector in m_ResourceActivitySelectorLookup.Values)
                {
                    selector.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            m_Disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Represents the initialization status of the SDK.
    /// </summary>
    public enum InitStatus
    {
        NotInitialized,
        Initializing,
        Success,
        Error
    }

    /// <summary>
    /// Global application state for the Qonversion SDK sample app.
    /// Manages all data and notifies listeners of changes.
    /// </summary>
    public class AppState
    {
        // Events for state changes
        public event Action OnStateChanged;
        public event Action<string> OnError;
        public event Action<string> OnSuccess;

        // Initialization status
        private InitStatus _initStatus = InitStatus.NotInitialized;
        public InitStatus InitStatus => _initStatus;

        // Loading state
        private bool _isLoading;
        public bool IsLoading => _isLoading;

        // User info
        private User _userInfo;
        public User UserInfo => _userInfo;

        // Products
        private Dictionary<string, Product> _products;
        public Dictionary<string, Product> Products => _products;

        // Offerings
        private Offerings _offerings;
        public Offerings Offerings => _offerings;

        // Entitlements
        private Dictionary<string, Entitlement> _entitlements;
        public Dictionary<string, Entitlement> Entitlements => _entitlements;

        // Remote configs
        private RemoteConfigList _remoteConfigList;
        public RemoteConfigList RemoteConfigList => _remoteConfigList;

        // User properties
        private UserProperties _userProperties;
        public UserProperties UserProperties => _userProperties;

        // Eligibilities
        private Dictionary<string, Eligibility> _eligibilities;
        public Dictionary<string, Eligibility> Eligibilities => _eligibilities;

        // No-Codes events log
        private readonly List<string> _noCodesEvents = new List<string>();
        public IReadOnlyList<string> NoCodesEvents => _noCodesEvents;

        // No-Codes initialization status
        private bool _noCodesInitialized;
        public bool NoCodesInitialized => _noCodesInitialized;

        // No-Codes purchase delegate enabled
        private bool _noCodesPurchaseDelegateEnabled;
        public bool NoCodesPurchaseDelegateEnabled => _noCodesPurchaseDelegateEnabled;

        // Setters with notification
        public void SetInitStatus(InitStatus status)
        {
            _initStatus = status;
            NotifyStateChanged();
        }

        public void SetLoading(bool loading)
        {
            _isLoading = loading;
            NotifyStateChanged();
        }

        public void SetUserInfo(User user)
        {
            _userInfo = user;
            NotifyStateChanged();
        }

        public void SetProducts(Dictionary<string, Product> products)
        {
            _products = products;
            NotifyStateChanged();
        }

        public void SetOfferings(Offerings offerings)
        {
            _offerings = offerings;
            NotifyStateChanged();
        }

        public void SetEntitlements(Dictionary<string, Entitlement> entitlements)
        {
            _entitlements = entitlements;
            NotifyStateChanged();
        }

        public void SetRemoteConfigList(RemoteConfigList configList)
        {
            _remoteConfigList = configList;
            NotifyStateChanged();
        }

        public void SetUserProperties(UserProperties properties)
        {
            _userProperties = properties;
            NotifyStateChanged();
        }

        public void SetEligibilities(Dictionary<string, Eligibility> eligibilities)
        {
            _eligibilities = eligibilities;
            NotifyStateChanged();
        }

        public void SetNoCodesInitialized(bool initialized)
        {
            _noCodesInitialized = initialized;
            NotifyStateChanged();
        }

        public void SetNoCodesPurchaseDelegateEnabled(bool enabled)
        {
            _noCodesPurchaseDelegateEnabled = enabled;
            NotifyStateChanged();
        }

        public void AddNoCodesEvent(string eventMessage)
        {
            _noCodesEvents.Add(eventMessage);
            NotifyStateChanged();
        }

        public void ClearNoCodesEvents()
        {
            _noCodesEvents.Clear();
            NotifyStateChanged();
        }

        public void ShowError(string message)
        {
            OnError?.Invoke(message);
        }

        public void ShowSuccess(string message)
        {
            OnSuccess?.Invoke(message);
        }

        private void NotifyStateChanged()
        {
            OnStateChanged?.Invoke();
        }
    }
}

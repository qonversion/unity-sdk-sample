using UnityEngine;
using UnityEngine.UIElements;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Controller for the Remote Configs screen.
    /// Displays remote configurations and experiments.
    /// </summary>
    public class RemoteConfigsScreenController : BaseScreenController
    {
        private VisualElement _configsContainer;
        private Label _emptyLabel;
        private TextField _contextKeyField;

        public RemoteConfigsScreenController(AppState appState) : base(appState) { }

        public override VisualElement CreateUI()
        {
            RootElement = new VisualElement();
            RootElement.style.flexGrow = 1;

            // Header
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 6;

            var title = new Label("Remote Configs");
            title.style.fontSize = 11;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            header.Add(title);

            var reloadButton = CreateButton("Load All", LoadAllConfigs);
            header.Add(reloadButton);

            RootElement.Add(header);

            // Context key input section
            var inputSection = CreateSection("Load by Context Key");
            
            var inputRow = new VisualElement();
            inputRow.style.flexDirection = FlexDirection.Row;
            inputRow.style.alignItems = Align.Center;

            _contextKeyField = new TextField();
            _contextKeyField.style.flexGrow = 1;
            _contextKeyField.style.marginRight = 4;
            _contextKeyField.style.fontSize = 8;
            _contextKeyField.value = "";
            var placeholder = new Label("Context key (empty = default)");
            placeholder.style.position = Position.Absolute;
            placeholder.style.left = 4;
            placeholder.style.top = 3;
            placeholder.style.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            placeholder.style.fontSize = 7;
            _contextKeyField.Add(placeholder);
            _contextKeyField.RegisterValueChangedCallback(evt => 
            {
                placeholder.style.display = string.IsNullOrEmpty(evt.newValue) ? DisplayStyle.Flex : DisplayStyle.None;
            });
            inputRow.Add(_contextKeyField);

            var loadButton = CreateButton("Load", LoadConfigByContextKey);
            inputRow.Add(loadButton);

            inputSection.Add(inputRow);
            RootElement.Add(inputSection);

            // Empty state
            _emptyLabel = new Label("No remote configs loaded. Click 'Load All' to fetch all configs.");
            _emptyLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            _emptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            _emptyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _emptyLabel.style.marginTop = 40;
            RootElement.Add(_emptyLabel);

            // Configs container
            var scrollView = CreateScrollView();
            _configsContainer = new VisualElement();
            scrollView.Add(_configsContainer);
            RootElement.Add(scrollView);

            Refresh();
            return RootElement;
        }

        public override void Refresh()
        {
            if (_configsContainer == null) return;
            _configsContainer.Clear();

            var configList = AppState.RemoteConfigList;

            if (configList?.RemoteConfigs == null || configList.RemoteConfigs.Count == 0)
            {
                _emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            _emptyLabel.style.display = DisplayStyle.None;

            foreach (var config in configList.RemoteConfigs)
            {
                var card = CreateConfigCard(config);
                _configsContainer.Add(card);
            }
        }

        private VisualElement CreateConfigCard(RemoteConfig config)
        {
            var card = CreateCard();

            // Header
            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.justifyContent = Justify.SpaceBetween;
            headerRow.style.alignItems = Align.Center;
            headerRow.style.marginBottom = 12;

            var contextKey = string.IsNullOrEmpty(config.Source?.ContextKey) ? "(default)" : config.Source.ContextKey;
            var titleLabel = new Label($"Context: {contextKey}");
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = Color.white;
            headerRow.Add(titleLabel);

            card.Add(headerRow);

            // Source info
            if (config.Source != null)
            {
                card.Add(CreateInfoRow("Source Type", config.Source.Type.ToString()));
                card.Add(CreateInfoRow("Source ID", config.Source.Id ?? "N/A"));
                card.Add(CreateInfoRow("Source Name", config.Source.Name ?? "N/A"));
            }

            // Experiment info
            if (config.Experiment != null)
            {
                var divider = new VisualElement();
                divider.style.height = 1;
                divider.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                divider.style.marginTop = 12;
                divider.style.marginBottom = 12;
                card.Add(divider);

                var experimentHeader = new Label("Experiment");
                experimentHeader.style.fontSize = 14;
                experimentHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                experimentHeader.style.color = new Color(0.8f, 0.5f, 0.9f, 1f);
                experimentHeader.style.marginBottom = 8;
                card.Add(experimentHeader);

                card.Add(CreateInfoRow("Experiment ID", config.Experiment.Id));
                card.Add(CreateInfoRow("Experiment Name", config.Experiment.Name));

                if (config.Experiment.Group != null)
                {
                    card.Add(CreateInfoRow("Group ID", config.Experiment.Group.Id));
                    card.Add(CreateInfoRow("Group Name", config.Experiment.Group.Name));
                    card.Add(CreateInfoRow("Group Type", config.Experiment.Group.Type.ToString()));
                }
            }

            // Payload
            if (config.Payload != null && config.Payload.Count > 0)
            {
                var divider = new VisualElement();
                divider.style.height = 1;
                divider.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                divider.style.marginTop = 12;
                divider.style.marginBottom = 12;
                card.Add(divider);

                var payloadHeader = new Label("Payload");
                payloadHeader.style.fontSize = 14;
                payloadHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                payloadHeader.style.color = new Color(0.9f, 0.7f, 0.2f, 1f);
                payloadHeader.style.marginBottom = 8;
                card.Add(payloadHeader);

                foreach (var kvp in config.Payload)
                {
                    card.Add(CreateInfoRow(kvp.Key, kvp.Value?.ToString() ?? "null"));
                }
            }

            return card;
        }

        private void LoadAllConfigs()
        {
            Debug.Log("🔄 [Qonversion] Loading all remote configs...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().RemoteConfigList((configList, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to load remote configs: {error.Message}");
                    AppState.ShowError($"Failed to load configs: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Remote configs loaded: {configList?.RemoteConfigs?.Count ?? 0}");
                AppState.SetRemoteConfigList(configList);
                AppState.ShowSuccess($"Loaded {configList?.RemoteConfigs?.Count ?? 0} configs");
            });
        }

        private void LoadConfigByContextKey()
        {
            var contextKey = _contextKeyField?.value;

            if (string.IsNullOrEmpty(contextKey))
            {
                Debug.Log("🔄 [Qonversion] Loading default remote config...");
                AppState.SetLoading(true);

                Qonversion.GetSharedInstance().RemoteConfig((config, error) =>
                {
                    AppState.SetLoading(false);

                    if (error != null)
                    {
                        Debug.LogError($"❌ [Qonversion] Failed to load config: {error.Message}");
                        AppState.ShowError($"Failed to load config: {error.Message}");
                        return;
                    }

                    Debug.Log("✅ [Qonversion] Default remote config loaded");
                    
                    // Show single config info
                    Debug.Log($"Config source: {config.Source?.ContextKey ?? "(default)"}");
                    AppState.ShowSuccess("Default config loaded - check Console for details");
                });
            }
            else
            {
                Debug.Log($"🔄 [Qonversion] Loading remote config for key: {contextKey}...");
                AppState.SetLoading(true);

                Qonversion.GetSharedInstance().RemoteConfig(contextKey, (config, error) =>
                {
                    AppState.SetLoading(false);

                    if (error != null)
                    {
                        Debug.LogError($"❌ [Qonversion] Failed to load config: {error.Message}");
                        AppState.ShowError($"Failed to load config: {error.Message}");
                        return;
                    }

                    Debug.Log($"✅ [Qonversion] Remote config loaded for key: {contextKey}");
                    
                    // Show single config info
                    Debug.Log($"Config source: {config.Source?.Name ?? "N/A"}");
                    AppState.ShowSuccess($"Config loaded for: {contextKey} - check Console for details");
                });
            }
        }
    }
}

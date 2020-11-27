﻿using System;
using System.Windows;
using System.Windows.Threading;
using HunterPie.Core;
using HunterPie.Core.Events;
using HunterPie.Core.Enums;
using System.Windows.Media;
using HunterPie.GUI.Widgets.HealthWidget.Parts;
using HunterPie.Core.Local;
using System.Collections.Generic;
using HunterPie.GUIControls.Custom_Controls;
using System.Windows.Shapes;
using System.Windows.Controls;
using HunterPie.GUI.Helpers;
using static HunterPie.Core.UserSettings.Config;

namespace HunterPie.GUI.Widgets.HealthWidget
{
    /// <summary>
    /// Interaction logic for PlayerHealth.xaml
    /// </summary>
    public partial class PlayerHealth : Widget
    {
        public new WidgetType Type => WidgetType.PlayerWidget;

        private Game gContext { get; set; }
        private Player Context => gContext.Player;

        private PlayerHealthComponent Settings => UserSettings.PlayerConfig.Overlay.PlayerHealthComponent;

        private const double SharpnessMaxWidth = 50;

        public string PlayerName
        {
            get { return (string)GetValue(PlayerNameProperty); }
            set { SetValue(PlayerNameProperty, value); }
        }
        public static readonly DependencyProperty PlayerNameProperty =
            DependencyProperty.Register("PlayerName", typeof(string), typeof(PlayerHealth));

        public string DayTimeIcon
        {
            get { return (string)GetValue(DayTimeIconProperty); }
            set { SetValue(DayTimeIconProperty, value); }
        }
        public static readonly DependencyProperty DayTimeIconProperty =
            DependencyProperty.Register("DayTimeIcon", typeof(string), typeof(PlayerHealth));

        public string PlayerLaurel
        {
            get { return (string)GetValue(PlayerLaurelProperty); }
            set { SetValue(PlayerLaurelProperty, value); }
        }
        public static readonly DependencyProperty PlayerLaurelProperty =
            DependencyProperty.Register("PlayerLaurel", typeof(string), typeof(PlayerHealth));

        public Brush SharpnessColor
        {
            get { return (Brush)GetValue(SharpnessColorProperty); }
            set { SetValue(SharpnessColorProperty, value); }
        }
        public static readonly DependencyProperty SharpnessColorProperty =
            DependencyProperty.Register("SharpnessColor", typeof(Brush), typeof(PlayerHealth));

        public double Sharpness
        {
            get { return (double)GetValue(SharpnessProperty); }
            set { SetValue(SharpnessProperty, value); }
        }
        public static readonly DependencyProperty SharpnessProperty =
            DependencyProperty.Register("Sharpness", typeof(double), typeof(PlayerHealth));

        public Visibility SharpnessVisibility
        {
            get { return (Visibility)GetValue(SharpnessVisibilityProperty); }
            set { SetValue(SharpnessVisibilityProperty, value); }
        }
        public static readonly DependencyProperty SharpnessVisibilityProperty =
            DependencyProperty.Register("SharpnessVisibility", typeof(Visibility), typeof(PlayerHealth));

        public bool IsStaminaNormal
        {
            get { return (bool)GetValue(IsStaminaNormalProperty); }
            set { SetValue(IsStaminaNormalProperty, value); }
        }
        public static readonly DependencyProperty IsStaminaNormalProperty =
            DependencyProperty.Register("IsStaminaNormal", typeof(bool), typeof(PlayerHealth));

        public bool IsWet
        {
            get { return (bool)GetValue(IsWetProperty); }
            set { SetValue(IsWetProperty, value); }
        }
        public static readonly DependencyProperty IsWetProperty =
            DependencyProperty.Register("IsWet", typeof(bool), typeof(PlayerHealth));

        public bool IsIcy
        {
            get { return (bool)GetValue(IsIcyProperty); }
            set { SetValue(IsIcyProperty, value); }
        }
        public static readonly DependencyProperty IsIcyProperty =
            DependencyProperty.Register("IsIcy", typeof(bool), typeof(PlayerHealth));

        public int PlayerCurrentStamina
        {
            get { return (int)GetValue(PlayerCurrentStaminaProperty); }
            set { SetValue(PlayerCurrentStaminaProperty, value); }
        }
        public static readonly DependencyProperty PlayerCurrentStaminaProperty =
            DependencyProperty.Register("PlayerCurrentStamina", typeof(int), typeof(PlayerHealth));

        public int PlayerMaxStamina
        {
            get { return (int)GetValue(PlayerMaxStaminaProperty); }
            set { SetValue(PlayerMaxStaminaProperty, value); }
        }
        public static readonly DependencyProperty PlayerMaxStaminaProperty =
            DependencyProperty.Register("PlayerMaxStamina", typeof(int), typeof(PlayerHealth));

        public int PlayerCurrentHealth
        {
            get { return (int)GetValue(PlayerCurrentHealthProperty); }
            set { SetValue(PlayerCurrentHealthProperty, value); }
        }
        public static readonly DependencyProperty PlayerCurrentHealthProperty =
            DependencyProperty.Register("PlayerCurrentHealth", typeof(int), typeof(PlayerHealth));

        public int PlayerMaxHealth
        {
            get { return (int)GetValue(PlayerMaxHealthProperty); }
            set { SetValue(PlayerMaxHealthProperty, value); }
        }
        public static readonly DependencyProperty PlayerMaxHealthProperty =
            DependencyProperty.Register("PlayerMaxHealth", typeof(int), typeof(PlayerHealth));

        public int PlayerCurrentSharpness
        {
            get { return (int)GetValue(PlayerCurrentSharpnessProperty); }
            set { SetValue(PlayerCurrentSharpnessProperty, value); }
        }
        public static readonly DependencyProperty PlayerCurrentSharpnessProperty =
            DependencyProperty.Register("PlayerCurrentSharpness", typeof(int), typeof(PlayerHealth));

        public int PlayerMaxSharpness
        {
            get { return (int)GetValue(PlayerMaxSharpnessProperty); }
            set { SetValue(PlayerMaxSharpnessProperty, value); }
        }
        public static readonly DependencyProperty PlayerMaxSharpnessProperty =
            DependencyProperty.Register("PlayerMaxSharpness", typeof(int), typeof(PlayerHealth));

        MinimalHealthBar StaminaBar { get; set; }
        HealthBar HealthBar { get; set; }
        Rectangle HealthExt { get; set; }
        Rectangle StaminaExt { get; set; }

        Panel AilmentDisplay { get; set; }
        AilmentControl ConstantAilment { get; set; }

        public PlayerHealth(Game ctx)
        {
            IsStaminaNormal = true;
            InitializeComponent();
            SetWindowFlags();
            SetContext(ctx);
        }

        public override void EnterWidgetDesignMode()
        {
            base.EnterWidgetDesignMode();
            RemoveWindowTransparencyFlag();
        }

        public override void LeaveWidgetDesignMode()
        {
            base.LeaveWidgetDesignMode();
            ApplyWindowTransparencyFlag();
            SaveSettings();
        }

        public override void ApplySettings(bool FocusTrigger = false)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Left = Settings.Position[0] + UserSettings.PlayerConfig.Overlay.Position[0];
                Top = Settings.Position[1] + UserSettings.PlayerConfig.Overlay.Position[1];

                WidgetActive = Settings.Enabled;

                PlayerName = FormatNameString();

                WidgetHasContent = Settings.HideHealthInVillages ? !(Context?.InHarvestZone ?? true) : true;

                Opacity = Settings.Opacity;

                ScaleWidget(Settings.Scale, Settings.Scale);
                base.ApplySettings(FocusTrigger);
            }));
        }

        private void SaveSettings()
        {
            Settings.Position = new int[2] { (int)Left - UserSettings.PlayerConfig.Overlay.Position[0], (int)Top - UserSettings.PlayerConfig.Overlay.Position[1] };

            Settings.Scale = DefaultScaleX;
        }

        public void SetContext(Game ctx)
        {
            gContext = ctx;
        }

        private void UpdateInformation()
        {
            OnMaxStaminaUpdate(this, new PlayerStaminaEventArgs(Context.Stamina));
            OnMaxHealthUpdate(this, new PlayerHealthEventArgs(Context.Health));
            if (Context.CurrentWeapon != null)
            {
                OnSharpnessLevelChange(this, new SharpnessEventArgs(Context.CurrentWeapon));
            }
        }

        private void HookEvents()
        {
            gContext.OnWorldDayTimeUpdate += OnWorldDayTimeUpdate;
            Context.OnZoneChange += OnZoneChange;
            Context.Health.OnHealthUpdate += OnHealthUpdate;
            Context.Health.OnMaxHealthUpdate += OnMaxHealthUpdate;
            Context.Health.OnHealHealth += OnHealHealth;
            Context.Health.OnRedHealthUpdate += OnRedHealthUpdate;
            Context.Health.OnHealthExtStateUpdate += OnHealthExtStateUpdate;
            Context.Stamina.OnStaminaUpdate += OnStaminaUpdate;
            Context.Stamina.OnMaxStaminaUpdate += OnMaxStaminaUpdate;
            Context.Stamina.OnStaminaExtStateUpdate += OnStaminaExtStateUpdate;
            Context.OnAilmentUpdate += OnAilmentUpdate;
            Context.OnLevelChange += OnLevelChange;

            if (Context.CurrentWeapon != null)
            {
                Context.CurrentWeapon.OnSharpnessChange += OnSharpnessChange;
                Context.CurrentWeapon.OnSharpnessLevelChange += OnSharpnessLevelChange;
            }

            Context.OnClassChange += OnClassChange;

            // To track debuffs
            Context.Abnormalities.OnNewAbnormality += OnNewAbnormality;
            Context.Abnormalities.OnAbnormalityRemove += OnAbnormalityEnd;
        }

        public void UnhookEvents()
        {
            gContext.OnWorldDayTimeUpdate -= OnWorldDayTimeUpdate;
            Context.OnZoneChange -= OnZoneChange;
            Context.Health.OnHealthUpdate -= OnHealthUpdate;
            Context.Health.OnMaxHealthUpdate -= OnMaxHealthUpdate;
            Context.Health.OnHealHealth -= OnHealHealth;
            Context.Health.OnRedHealthUpdate -= OnRedHealthUpdate;
            Context.Health.OnHealthExtStateUpdate -= OnHealthExtStateUpdate;
            Context.Stamina.OnStaminaUpdate -= OnStaminaUpdate;
            Context.Stamina.OnMaxStaminaUpdate -= OnMaxStaminaUpdate;
            Context.Stamina.OnStaminaExtStateUpdate -= OnStaminaExtStateUpdate;
            Context.OnAilmentUpdate -= OnAilmentUpdate;
            Context.OnLevelChange -= OnLevelChange;

            if (Context.CurrentWeapon != null)
            {
                Context.CurrentWeapon.OnSharpnessChange -= OnSharpnessChange;
                Context.CurrentWeapon.OnSharpnessLevelChange -= OnSharpnessLevelChange;
            }

            Context.OnClassChange -= OnClassChange;

            Context.Abnormalities.OnNewAbnormality -= OnNewAbnormality;
            Context.Abnormalities.OnAbnormalityRemove -= OnAbnormalityEnd;
        }


        private void OnZoneChange(object source, EventArgs args)
        {
            PlayerLocationEventArgs e = (PlayerLocationEventArgs)args;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                WidgetHasContent = !e.InHarvestZone && Settings.HideHealthInVillages;
                ChangeVisibility(false);
            }));
        }

        private void OnAbnormalityEnd(object source, AbnormalityEventArgs args)
        {
            HashSet<string> poison = new HashSet<string>()
            {
                "ICON_POISON", "ICON_VENOM"
            };
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                string abnormIcon = args.Abnormality.Icon;

                if (args.Abnormality.IsDebuff)
                {
                    if (abnormIcon == "ELEMENT_FIRE") HealthBar.IsOnFire = false;
                    else if (abnormIcon == "ICON_BLEED") HealthBar.IsBleeding = false;
                    else if (abnormIcon == "ICON_EFFLUVIA") HealthBar.HasEffluvia = false;
                    else if (abnormIcon == "ELEMENT_WATER") IsWet = false;
                    else if (abnormIcon == "ELEMENT_ICE") IsIcy = false;
                    else if (poison.Contains(abnormIcon)) HealthBar.IsPoisoned = false;
                    
                    HealthBar.IsNormal = (!HealthBar.IsOnFire && !HealthBar.IsPoisoned && !HealthBar.IsBleeding && !HealthBar.HasEffluvia);
                    IsStaminaNormal = (!IsWet && !IsIcy);
                } else
                {
                    HealthBar.IsHealing = !(abnormIcon == "ICON_NATURALHEALING");
                }
            }));
        }

        private void OnNewAbnormality(object source, AbnormalityEventArgs args)
        {
            HashSet<string> poison = new HashSet<string>()
            {
                "ICON_POISON", "ICON_VENOM"
            };
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                string abnormIcon = args.Abnormality.Icon;

                if (args.Abnormality.IsDebuff)
                {

                    if (abnormIcon == "ELEMENT_FIRE") HealthBar.IsOnFire = true;
                    else if (abnormIcon == "ICON_BLEED") HealthBar.IsBleeding = true;
                    else if (abnormIcon == "ICON_EFFLUVIA") HealthBar.HasEffluvia = true;
                    else if (abnormIcon == "ELEMENT_WATER") IsWet = true;
                    else if (abnormIcon == "ELEMENT_ICE") IsIcy = true;
                    else if (poison.Contains(abnormIcon)) HealthBar.IsPoisoned = true;

                } else
                {
                    HealthBar.IsHealing = abnormIcon == "ICON_NATURALHEALING";
                }
            }));
        }

        private void OnHealthExtStateUpdate(object source, PlayerHealthEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (args.IsHealthExtVisible && args.MaxPossibleHealth != args.MaxHealth)
                {
                    HealthExt.Visibility = Visibility.Visible;

                    float maxHealth = Math.Min(args.MaxHealth + HealthComponent.CanIncreaseMaxHealth[args.SelectedItemId], args.MaxPossibleHealth);
                    HealthExt.Width = (maxHealth - args.MaxHealth) / args.MaxPossibleHealth * HealthBar.ConstantWidth * (HealthBar.ConstantWidth / HealthBar.CHealth);
                } else
                {
                    HealthExt.Width = 0;
                    HealthExt.Visibility = Visibility.Collapsed;
                }
            }));
        }

        private void OnStaminaExtStateUpdate(object source, PlayerStaminaEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (args.IsStaminaExtVisible && args.MaxPossibleStamina != args.MaxStamina)
                {
                    StaminaExt.Visibility = Visibility.Visible;
                    float maxStamina = Math.Min(args.MaxStamina + StaminaComponent.CanIncreaseMaxStamina[args.SelectedItemId], args.MaxPossibleStamina);
                    StaminaExt.Width = (maxStamina - args.MaxStamina) / args.MaxPossibleStamina * HealthBar.ConstantWidth * (HealthBar.ConstantWidth / HealthBar.CHealth);
                } else
                {
                    StaminaExt.Width = 0;
                    StaminaExt.Visibility = Visibility.Collapsed;
                }
            }));
        }

        private void OnWorldDayTimeUpdate(object source, WorldEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                string iconPath = "pack://siteoforigin:,,,/HunterPie.Resources/UI/HUD/";
                switch (args.DayTime)
                {
                    case DayTime.Morning:
                        DayTimeIcon = $"{iconPath}mr_time_morning.png";
                        break;
                    case DayTime.Afternoon:
                        DayTimeIcon = $"{iconPath}mr_time_day.png";
                        break;
                    case DayTime.Evening:
                        DayTimeIcon = $"{iconPath}mr_time_evening.png";
                        break;
                    case DayTime.Night:
                        DayTimeIcon = $"{iconPath}mr_time_night.png";
                        break;
                    default:
                        DayTimeIcon = null;
                        break;
                }
            }));
        }

        private void OnClassChange(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (Context.LastWeapon != null)
                {
                    Context.LastWeapon.OnSharpnessChange -= OnSharpnessChange;
                    Context.LastWeapon.OnSharpnessLevelChange -= OnSharpnessLevelChange;
                }
                // To avoid hooking twice
                Context.CurrentWeapon.OnSharpnessChange -= OnSharpnessChange;
                Context.CurrentWeapon.OnSharpnessLevelChange -= OnSharpnessLevelChange;
                Context.CurrentWeapon.OnSharpnessChange += OnSharpnessChange;
                Context.CurrentWeapon.OnSharpnessLevelChange += OnSharpnessLevelChange;

                SharpnessVisibility = Context.CurrentWeapon.IsMelee ? Visibility.Visible : Visibility.Collapsed;

                OnSharpnessLevelChange(this, new SharpnessEventArgs(Context.CurrentWeapon));
            }));
        }

        private void OnSharpnessLevelChange(object source, SharpnessEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                object color = TryFindResource($"SHARPNESS_{(args.Level).ToString().ToUpperInvariant()}");

                if (color != null)
                {
                    SharpnessColor = color as Brush;
                }

                int min = Math.Min(args.MaximumSharpness, args.Max);

                PlayerCurrentSharpness = args.Sharpness;
                PlayerMaxSharpness = min;

                Sharpness = ((args.Sharpness - args.Min) / (double)(min - args.Min)) * SharpnessMaxWidth;
            }));
        }

        private void OnSharpnessChange(object source, SharpnessEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                int min = Math.Min(args.MaximumSharpness, args.Max);

                PlayerCurrentSharpness = args.Sharpness;
                PlayerMaxSharpness = min;

                Sharpness = ((args.Sharpness - args.Min) / (double)(min - args.Min)) * SharpnessMaxWidth;
            }));
        }

        private void OnLevelChange(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                PlayerEventArgs e = (PlayerEventArgs)args;
                // We update the name string
                PlayerName = FormatNameString();
                // And the laurel
                string laurel = "pack://siteoforigin:,,,/HunterPie.Resources/UI/HUD/";

                switch (Player.GetLaurelFromLevel(e.MasterRank))
                {
                    case Laurel.Copper:
                        laurel += "mr_laurel_copper";
                        break;
                    case Laurel.Silver:
                        laurel += "mr_laurel_silver";
                        break;
                    case Laurel.Gold:
                        laurel += "mr_laurel_gold";
                        break;
                    case Laurel.Diamond:
                        laurel += "mr_laurel_diamond";
                        break;
                    default:
                        laurel += "mr_laurel_iron";
                        break;
                }
                laurel += ".png";
                PlayerLaurel = laurel;
            }));
        }

        private string FormatNameString()
        {
            if (Context is null)
            {
                return null;
            }

            return Settings.NameTextFormat.Replace("{HR}", Context.Level.ToString())
                .Replace("{MR}", Context.MasterRank.ToString())
                .Replace("{Name}", Context.Name);

        }

        private void OnAilmentUpdate(object source, PlayerAilmentEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (args.AilmentType == PlayerAilment.None)
                {
                    ConstantAilment.Icon = null;
                    ConstantAilment.Visibility = Visibility.Collapsed;
                    return;
                } else
                {
                    ConstantAilment.Visibility = Visibility.Visible;
                    ImageSource icon = TryFindResource($"ICON_{args.AilmentType.ToString().ToUpperInvariant()}") as ImageSource;
                    ConstantAilment.Icon = icon;
                }

                ConstantAilment.TimerEndAngle = Arc.ConvertPercentageIntoAngle(args.AilmentTimer / Math.Max(1, args.AilmentMaxTimer));
            }));
        }

        private void OnMaxStaminaUpdate(object source, PlayerStaminaEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                PlayerCurrentStamina = (int)args.Stamina;
                PlayerMaxStamina = (int)args.MaxStamina;

                StaminaBar.MaxWidth = args.MaxStamina / 100 * HealthBar.ConstantWidth;
                StaminaBar.MaxSize = args.MaxStamina / 100 * HealthBar.ConstantWidth;

                OnStaminaExtStateUpdate(source, args);
            }));
        }

        private void OnStaminaUpdate(object source, PlayerStaminaEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                PlayerCurrentStamina = (int)args.Stamina;
                PlayerMaxStamina = (int)args.MaxStamina;

                StaminaBar.UpdateBar(args.Stamina, args.MaxStamina);
            }));
        }

        private void OnRedHealthUpdate(object source, PlayerHealthEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                HealthBar.RedHealth = args.RedHealth;
            }));
        }

        private void OnHealHealth(object source, PlayerHealthEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                double healValue = args.HealingData.Stage == 1 ?
                    args.HealingData.MaxHeal * 2.5 : args.HealingData.MaxHeal;

                if (args.HealingData.Stage == 0)
                {
                    HealthBar.HealHealth = 0; 
                } else
                {
                    HealthBar.HealHealth = Math.Min(args.Health + (healValue - args.HealingData.CurrentHeal), args.MaxHealth);
                }
            }));
        }

        private void OnMaxHealthUpdate(object source, PlayerHealthEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                PlayerCurrentHealth = (int)args.Health;
                PlayerMaxHealth = (int)args.MaxHealth;

                HealthBar.MaxHealth = args.MaxHealth;
                HealthBar.Health = args.Health;

                OnHealthExtStateUpdate(source, args);
            }));
        }

        private void OnHealthUpdate(object source, PlayerHealthEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                PlayerCurrentHealth = (int)args.Health;
                PlayerMaxHealth = (int)args.MaxHealth;

                HealthBar.MaxHealth = args.MaxHealth;
                HealthBar.Health = args.Health;
                if (args.RedHealth > 0)
                {
                    HealthBar.RedHealth = args.RedHealth;
                }
            }));
        }

        public override void ScaleWidget(double NewScaleX, double NewScaleY)
        {
            if (NewScaleX <= 0.2) return;
            FrameworkElement panel = (FrameworkElement)Template.FindName("HudPanel", this);
            if (panel is null)
            {
                return;
            }
            panel.LayoutTransform = new ScaleTransform(NewScaleX, NewScaleY);
            DefaultScaleX = NewScaleX;
            DefaultScaleY = NewScaleY;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnhookEvents();
            IsClosed = true;
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                MoveWidget();
                SaveSettings();
            }
        }

        private void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ScaleWidget(DefaultScaleX + 0.05, DefaultScaleY + 0.05);
            }
            else
            {
                ScaleWidget(DefaultScaleX - 0.05, DefaultScaleY - 0.05);
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            StaminaBar = Template.FindName("StaminaBar", this) as MinimalHealthBar;
            HealthBar = Template.FindName("HealthBar", this) as HealthBar;
            HealthExt = Template.FindName("HealthExt", this) as Rectangle;
            StaminaExt = Template.FindName("StaminaExt", this) as Rectangle;

            AilmentDisplay = Template.FindName("AilmentDisplay", this) as Panel;
            ConstantAilment = Template.FindName("ConstantAilment", this) as AilmentControl;

            HookEvents();
            UpdateInformation();
            ApplySettings();
        }
    }
}

﻿using System;
using System.Windows;
using System.Windows.Threading;
using HunterPie.Core;
using HunterPie.Core.Events;
using HunterPie.Core.Enums;

namespace HunterPie.GUI.Widgets.HealthWidget
{
    /// <summary>
    /// Interaction logic for PlayerHealth.xaml
    /// </summary>
    public partial class PlayerHealth : Widget
    {
        private Game gContext { get; set; }
        private Player Context => gContext.Player;

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


        public PlayerHealth(Game ctx)
        {
            WidgetType = 7;
            WidgetActive = true;
            WidgetHasContent = true;
            InitializeComponent();
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
                base.ApplySettings(FocusTrigger);
            }));
        }

        private void SaveSettings()
        {

        }

        public void SetContext(Game ctx)
        {
            gContext = ctx;
            HookEvents();
            UpdateInformation();
        }

        private void UpdateInformation()
        {
            OnMaxStaminaUpdate(this, new PlayerStaminaEventArgs(Context));
            OnMaxHealthUpdate(this, new PlayerHealthEventArgs(Context));
        }

        private void HookEvents()
        {
            gContext.OnWorldDayTimeUpdate += OnWorldDayTimeUpdate;
            Context.OnHealthUpdate += OnHealthUpdate;
            Context.OnMaxHealthUpdate += OnMaxHealthUpdate;
            Context.OnHealHealth += OnHealHealth;
            Context.OnRedHealthUpdate += OnRedHealthUpdate;
            Context.OnStaminaUpdate += OnStaminaUpdate;
            Context.OnMaxStaminaUpdate += OnMaxStaminaUpdate;
            Context.OnAilmentUpdate += OnAilmentUpdate;
            Context.OnLevelChange += OnLevelChange;
        }

        public void UnhookEvents()
        {
            gContext.OnWorldDayTimeUpdate -= OnWorldDayTimeUpdate;
            Context.OnHealthUpdate -= OnHealthUpdate;
            Context.OnMaxHealthUpdate -= OnMaxHealthUpdate;
            Context.OnHealHealth -= OnHealHealth;
            Context.OnRedHealthUpdate -= OnRedHealthUpdate;
            Context.OnStaminaUpdate -= OnStaminaUpdate;
            Context.OnMaxStaminaUpdate -= OnMaxStaminaUpdate;
            Context.OnAilmentUpdate -= OnAilmentUpdate;
            Context.OnLevelChange += OnLevelChange;
        }

        private void OnWorldDayTimeUpdate(object source, WorldEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                switch (args.DayTime)
                {
                    case DayTime.Morning:
                        DayTimeIcon = "pack://siteoforigin:,,,/HunterPie.Resources/UI/HUD/mr_time_morning.png";
                        break;
                    case DayTime.Afternoon:
                        DayTimeIcon = "pack://siteoforigin:,,,/HunterPie.Resources/UI/HUD/mr_time_day.png";
                        break;
                    case DayTime.Evening:
                        DayTimeIcon = "pack://siteoforigin:,,,/HunterPie.Resources/UI/HUD/mr_time_evening.png";
                        break;
                    case DayTime.Night:
                        DayTimeIcon = "pack://siteoforigin:,,,/HunterPie.Resources/UI/HUD/mr_time_night.png";
                        break;
                    default:
                        DayTimeIcon = null;
                        break;
                }
            }));
        }

        private void OnLevelChange(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                PlayerEventArgs e = (PlayerEventArgs)args;
                // We update the name string
                PlayerName = $"Lv. {e.Level} {e.Name}";
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

        private void OnAilmentUpdate(object source, PlayerAilmentEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {

            }));
        }

        private void OnMaxStaminaUpdate(object source, PlayerStaminaEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                StaminaBar.MaxWidth = args.MaxStamina / 100 * 200;
                StaminaBar.MaxSize = args.MaxStamina / 100 * 200;
            }));
        }

        private void OnStaminaUpdate(object source, PlayerStaminaEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
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
                HealthBar.MaxHealth = args.MaxHealth;
                HealthBar.Health = args.Health;
            }));
        }

        private void OnHealthUpdate(object source, PlayerHealthEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                HealthBar.MaxHealth = args.MaxHealth;
                HealthBar.Health = args.Health;
                if (args.RedHealth > 0)
                {
                    HealthBar.RedHealth = args.RedHealth;
                }
            }));
        }

        public void ScaleWidget(double NewScaleX, double NewScaleY)
        {
            
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

    }
}

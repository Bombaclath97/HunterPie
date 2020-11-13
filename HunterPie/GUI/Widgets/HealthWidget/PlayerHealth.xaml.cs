﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HunterPie.Core;
using HunterPie.Logger;
using HunterPie.Core.Events;
using Newtonsoft.Json;
using Microsoft.SqlServer.Server;

namespace HunterPie.GUI.Widgets.HealthWidget
{
    /// <summary>
    /// Interaction logic for PlayerHealth.xaml
    /// </summary>
    public partial class PlayerHealth : Widget
    {

        private Player Context { get; set; }

        public PlayerHealth(Player ctx)
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

        public void SetContext(Player ctx)
        {
            Context = ctx;
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
            Context.OnHealthUpdate += OnHealthUpdate;
            Context.OnMaxHealthUpdate += OnMaxHealthUpdate;
            Context.OnHealHealth += OnHealHealth;
            Context.OnRedHealthUpdate += OnRedHealthUpdate;
            Context.OnStaminaUpdate += OnStaminaUpdate;
            Context.OnMaxStaminaUpdate += OnMaxStaminaUpdate;
        }

        public void UnhookEvents()
        {
            Context.OnHealthUpdate -= OnHealthUpdate;
            Context.OnMaxHealthUpdate -= OnMaxHealthUpdate;
            Context.OnHealHealth -= OnHealHealth;
            Context.OnRedHealthUpdate -= OnRedHealthUpdate;
            Context.OnStaminaUpdate -= OnStaminaUpdate;
            Context.OnMaxStaminaUpdate -= OnMaxStaminaUpdate;
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

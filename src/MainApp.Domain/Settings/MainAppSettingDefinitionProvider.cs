﻿using Volo.Abp.Settings;

namespace MainApp.Settings
{
    public class MainAppSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(MainAppSettings.MySetting1));
        }
    }
}

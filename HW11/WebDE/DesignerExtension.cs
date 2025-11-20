using DocsVision.Platform.Tools.LayoutEditor.Extensibility;
using DocsVision.Platform.Tools.LayoutEditor.Helpers;
using DocsVision.Platform.Tools.LayoutEditor.Infrostructure;
using DocsVision.Platform.Tools.LayoutEditor.ObjectModel.Descriptions;
using System;
using System.Collections.Generic;
using System.Resources;

namespace WebDEX.Extension
{
    /// <summary>
    /// Представляет собой пример расширения для редактора разметок
    /// </summary>
    class WebDEX : WebLayoutsDesignerExtension
    {
        /// <summary>
        /// Создаёт новый экземпляр <see cref="WebDEX"/>
        /// </summary>
        /// <param name="provider">Сервис-провайдер</param>
        public WebDEX(IServiceProvider provider)
            : base(provider)
        {
        }

        /// <summary>
        /// Возвращает словарь ключ/описание свойства, которые будут использоваться в пользовательских контролах
        /// </summary>
        protected override Dictionary<string, PropertyDescription> GetPropertyDescriptions()
        {
            var result = new Dictionary<string, PropertyDescription>();
            var cityCtrl = new PropertyDescription
            {
                Name = "CityControl",
                Type = typeof(string),
                Category = PropertyCategoryConstants.DataCategory,
                DisplayName = "Контрол города"
            };

            result.Add(cityCtrl.Name, cityCtrl);

            return result;
        }

        /// <summary>
        /// Возвращает описание пользовательских контролов
        /// описание контрола PartnerLink выполнено альтернативным способом и содержится в каталоге xml
        /// </summary>
        protected override List<ControlTypeDescription> GetControlTypeDescriptions()
        {
            return new List<ControlTypeDescription>
            {
                GetAviasalesControlDescription()
            };
        }

        /// <summary>
        /// Возвращает ResourceManager этого расширения (расширяет словарь локализации конструктора разметок, не путать с окном Localization конструктора разметок)
        /// </summary>
        protected override List<ResourceManager> GetResourceManagers()
        {
            return new List<ResourceManager>
            {
                Resources.ResourceManager
            };
        }

        private ControlTypeDescription GetAviasalesControlDescription()
        {
            var desc = new ControlTypeDescription(Constants.ClassName)
            {
                DisplayName = "Запрос стоимости билетов",  
                ControlGroupDisplayName = "API запросы",
                IsBlockLevel = true
            };

            desc.PropertyDescriptions.Add(PropertyFactory.GetNameProperty());    
            desc.PropertyDescriptions.Add(PropertyFactory.GetLabelTextProperty());   
            desc.PropertyDescriptions.Add(PropertyFactory.GetVisibilityProperty());  
            desc.PropertyDescriptions.Add(PropertyDescriptions["CityControl"]);

            return desc;
        }
    }
}

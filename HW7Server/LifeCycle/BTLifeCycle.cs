using DocsVision.Platform.WebClient;
using DocsVision.WebClientLibrary.ObjectModel.Services.EntityLifeCycle;
using DocsVision.WebClientLibrary.ObjectModel.Services.EntityLifeCycle.Options;
using ServerExtension.CardEditor.Services;
using System;
using System.Collections.Generic;

namespace ServerExtension.CardEditor.LifeCycle
{
    public class BTLifeCycle : ICardLifeCycleEx
    {
        private readonly ICardLifeCycleEx inner;
        private readonly IBTService btService;

        public BTLifeCycle(ICardLifeCycleEx inner, IBTService btService)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.btService = btService ?? throw new ArgumentNullException(nameof(btService));
        }

        public Guid CardTypeId => inner.CardTypeId;

        public Guid Create(SessionContext sessionContext, CardCreateLifeCycleOptions options)
        {
            var cardId = inner.Create(sessionContext, options);

            if (cardId != Guid.Empty)
                btService.InitializeOnCreate(sessionContext, cardId);

            return cardId;
        }

        public bool Validate(SessionContext sessionContext, CardValidateLifeCycleOptions options,
            out List<ValidationResult> validationResults)
        {
            return inner.Validate(sessionContext, options, out validationResults);
        }

        public void OnSave(SessionContext sessionContext, CardSaveLifeCycleOptions options)
        {
            inner.OnSave(sessionContext, options);
        }

        public bool CanDelete(SessionContext sessionContext, CardDeleteLifeCycleOptions options, out string message)
        {
            return inner.CanDelete(sessionContext, options, out message);
        }

        public void OnDelete(SessionContext sessionContext, CardDeleteLifeCycleOptions options)
        {
            inner.OnDelete(sessionContext, options);
        }

        public string GetDigest(SessionContext sessionContext, CardDigestLifeCycleOptions options)
        {
            return inner.GetDigest(sessionContext, options);
        }
    }
}

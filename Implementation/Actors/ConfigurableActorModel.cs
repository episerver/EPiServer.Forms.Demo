using EPiServer.Forms.Core.PostSubmissionActor;
using System;
using System.ComponentModel.DataAnnotations;


namespace EPiServer.Forms.Demo.Implementation.Actors
{
    /// <summary>
    /// This will be the model for Actor config, it is a row in the Actor configuration UI in the EditView
    /// </summary>
    [Serializable]
    public class ConfigurableActorModel : IPostSubmissionActorModel, ICloneable
    {
        [Display(
            Name = "Config Key",
            Order = 1000)]
        public virtual string ConfigKey { get; set; }
        [Display(
            Name = "Config Value",
            Order = 1100)]
        public virtual string ConfigValue { get; set; }


        #region ICloneable Members

        public object Clone()
        {
            return new ConfigurableActorModel
            {
                ConfigKey = this.ConfigKey,
                ConfigValue = this.ConfigValue,
            };
        }

        #endregion
    }
}

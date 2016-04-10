﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheBox.Common.TVListingService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TVSchedule", Namespace="http://schemas.datacontract.org/2004/07/TVListingsAPI")]
    [System.SerializableAttribute()]
    public partial class TVSchedule : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private TheBox.Common.TVListingService.TVProvider[] ProvidersField;
        
        private System.DateTime ScheduleDateField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public TheBox.Common.TVListingService.TVProvider[] Providers {
            get {
                return this.ProvidersField;
            }
            set {
                if ((object.ReferenceEquals(this.ProvidersField, value) != true)) {
                    this.ProvidersField = value;
                    this.RaisePropertyChanged("Providers");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.DateTime ScheduleDate {
            get {
                return this.ScheduleDateField;
            }
            set {
                if ((this.ScheduleDateField.Equals(value) != true)) {
                    this.ScheduleDateField = value;
                    this.RaisePropertyChanged("ScheduleDate");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TVProvider", Namespace="http://schemas.datacontract.org/2004/07/TVListingsAPI")]
    [System.SerializableAttribute()]
    public partial class TVProvider : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private TheBox.Common.TVListingService.Channel[] ChannelsField;
        
        private string ProviderNameField;
        
        private string ServiceIdField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public TheBox.Common.TVListingService.Channel[] Channels {
            get {
                return this.ChannelsField;
            }
            set {
                if ((object.ReferenceEquals(this.ChannelsField, value) != true)) {
                    this.ChannelsField = value;
                    this.RaisePropertyChanged("Channels");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string ProviderName {
            get {
                return this.ProviderNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ProviderNameField, value) != true)) {
                    this.ProviderNameField = value;
                    this.RaisePropertyChanged("ProviderName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string ServiceId {
            get {
                return this.ServiceIdField;
            }
            set {
                if ((object.ReferenceEquals(this.ServiceIdField, value) != true)) {
                    this.ServiceIdField = value;
                    this.RaisePropertyChanged("ServiceId");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Channel", Namespace="http://schemas.datacontract.org/2004/07/TVListingsAPI")]
    [System.SerializableAttribute()]
    public partial class Channel : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string ChannelNumberField;
        
        private string NameField;
        
        private TheBox.Common.TVListingService.Programme[] ProgrammesField;
        
        private string ServiceIdField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string ChannelNumber {
            get {
                return this.ChannelNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.ChannelNumberField, value) != true)) {
                    this.ChannelNumberField = value;
                    this.RaisePropertyChanged("ChannelNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public TheBox.Common.TVListingService.Programme[] Programmes {
            get {
                return this.ProgrammesField;
            }
            set {
                if ((object.ReferenceEquals(this.ProgrammesField, value) != true)) {
                    this.ProgrammesField = value;
                    this.RaisePropertyChanged("Programmes");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string ServiceId {
            get {
                return this.ServiceIdField;
            }
            set {
                if ((object.ReferenceEquals(this.ServiceIdField, value) != true)) {
                    this.ServiceIdField = value;
                    this.RaisePropertyChanged("ServiceId");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Programme", Namespace="http://schemas.datacontract.org/2004/07/TVListingsAPI")]
    [System.SerializableAttribute()]
    public partial class Programme : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string ChannelNamek__BackingFieldField;
        
        private string ChannelNumberk__BackingFieldField;
        
        private string ProgramNamek__BackingFieldField;
        
        private string ShowingTimek__BackingFieldField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<ChannelName>k__BackingField", IsRequired=true)]
        public string ChannelNamek__BackingField {
            get {
                return this.ChannelNamek__BackingFieldField;
            }
            set {
                if ((object.ReferenceEquals(this.ChannelNamek__BackingFieldField, value) != true)) {
                    this.ChannelNamek__BackingFieldField = value;
                    this.RaisePropertyChanged("ChannelNamek__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<ChannelNumber>k__BackingField", IsRequired=true)]
        public string ChannelNumberk__BackingField {
            get {
                return this.ChannelNumberk__BackingFieldField;
            }
            set {
                if ((object.ReferenceEquals(this.ChannelNumberk__BackingFieldField, value) != true)) {
                    this.ChannelNumberk__BackingFieldField = value;
                    this.RaisePropertyChanged("ChannelNumberk__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<ProgramName>k__BackingField", IsRequired=true)]
        public string ProgramNamek__BackingField {
            get {
                return this.ProgramNamek__BackingFieldField;
            }
            set {
                if ((object.ReferenceEquals(this.ProgramNamek__BackingFieldField, value) != true)) {
                    this.ProgramNamek__BackingFieldField = value;
                    this.RaisePropertyChanged("ProgramNamek__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<ShowingTime>k__BackingField", IsRequired=true)]
        public string ShowingTimek__BackingField {
            get {
                return this.ShowingTimek__BackingFieldField;
            }
            set {
                if ((object.ReferenceEquals(this.ShowingTimek__BackingFieldField, value) != true)) {
                    this.ShowingTimek__BackingFieldField = value;
                    this.RaisePropertyChanged("ShowingTimek__BackingField");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TVListingService.ITVListingService")]
    public interface ITVListingService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITVListingService/GetTVSchedule", ReplyAction="http://tempuri.org/ITVListingService/GetTVScheduleResponse")]
        TheBox.Common.TVListingService.TVSchedule GetTVSchedule();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITVListingService/GetTVSchedule", ReplyAction="http://tempuri.org/ITVListingService/GetTVScheduleResponse")]
        System.Threading.Tasks.Task<TheBox.Common.TVListingService.TVSchedule> GetTVScheduleAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITVListingServiceChannel : TheBox.Common.TVListingService.ITVListingService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TVListingServiceClient : System.ServiceModel.ClientBase<TheBox.Common.TVListingService.ITVListingService>, TheBox.Common.TVListingService.ITVListingService {
        
        public TVListingServiceClient() {
        }
        
        public TVListingServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TVListingServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TVListingServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TVListingServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public TheBox.Common.TVListingService.TVSchedule GetTVSchedule() {
            return base.Channel.GetTVSchedule();
        }
        
        public System.Threading.Tasks.Task<TheBox.Common.TVListingService.TVSchedule> GetTVScheduleAsync() {
            return base.Channel.GetTVScheduleAsync();
        }
    }
}

﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ServiceClientModel = Microsoft.Azure.Management.RecoveryServices.Backup.Models;
using Microsoft.Azure.Commands.RecoveryServices.Backup.Properties;

namespace Microsoft.Azure.Commands.RecoveryServices.Backup.Cmdlets.Models
{
    public class ObjectBase
    {
        public virtual void Validate() { }
    }

    public class ManagementContext : ObjectBase
    {
        /// <summary>
        /// BackupManagementType
        /// </summary>
        public BackupManagementType BackupManagementType { get; set; }

        public ManagementContext() { }

        public ManagementContext(string backupManagementType)
        {
            BackupManagementType = ConversionUtils.GetPsBackupManagementType(backupManagementType);
        }
    }

    public class ContainerContext : ManagementContext
    {
        public ContainerType ContainerType { get; set; }

        public ContainerContext() { }

        public ContainerContext(ContainerType containerType, string backupManagementType)
            : base(backupManagementType)
        {
            ContainerType = containerType;
        }

        public ContainerContext(string backupManagementType)
            : base(backupManagementType)
        {
            
        }
    }

    public class AzureRmRecoveryServicesBackupEngineContext : ManagementContext
    {
        public string BackupEngineType { get; set; }

        public AzureRmRecoveryServicesBackupEngineContext() { }

        public AzureRmRecoveryServicesBackupEngineContext(string backupEngineType, string backupManagementType)
            : base(backupManagementType)
        {
            BackupEngineType = backupEngineType;
        }
    }

    public class ContainerBase : ContainerContext
    {
        /// <summary>
        /// Container Name
        /// </summary>
        public string Name { get; set; }

        public ContainerBase(ServiceClientModel.ProtectionContainerResource protectionContainer)
            : base(ConversionUtils.GetPsContainerType(((ServiceClientModel.ProtectionContainer)protectionContainer.Properties).ContainerType),
                   ((ServiceClientModel.ProtectionContainer)protectionContainer.Properties).BackupManagementType)
        {
            Name = IdUtils.GetNameFromUri(protectionContainer.Name);
        }
    }

    public class BackupEngineBase : AzureRmRecoveryServicesBackupEngineContext
    {
        /// <summary>
        /// Container Name
        /// </summary>
        public string Name { get; set; }

        public BackupEngineBase(ServiceClientModel.BackupEngineResource backupEngine)
            : base(((ServiceClientModel.BackupEngineBase)backupEngine.Properties).BackupEngineType,
                   ((ServiceClientModel.BackupEngineBase)backupEngine.Properties).BackupManagementType)
        {
            Name = backupEngine.Name;
        }
    }

    /// <summary>
    /// Represents Azure Backup Item Context Class
    /// </summary>
    public class ItemContext : ContainerContext
    {
        /// <summary>
        /// Workload Type of Item
        /// </summary>
        public WorkloadType WorkloadType { get; set; }

        /// <summary>
        /// Unique name of the Container
        /// </summary>
        public string ContainerName { get; set; }

        public ItemContext()
            : base()
        {

        }

        public ItemContext(ServiceClientModel.ProtectedItem protectedItem,
            string containerName, ContainerType containerType)
            : base(containerType, protectedItem.BackupManagementType)
        {
            WorkloadType = ConversionUtils.GetPsWorkloadType(protectedItem.WorkloadType);
            ContainerName = containerName;
        }
    }

    /// <summary>
    /// Represents Azure Backup Item Base Class
    /// </summary>
    public class ItemBase : ItemContext
    {
        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of the item
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Last Recovery Point for the item
        /// </summary>
        public DateTime? LatestRecoveryPoint { get; set; }

        public ItemBase(ServiceClientModel.ProtectedItemResource protectedItemResource,
            string containerName, ContainerType containerType)
            : base((ServiceClientModel.ProtectedItem)protectedItemResource.Properties, containerName, containerType)
        {
            ServiceClientModel.ProtectedItem protectedItem = (ServiceClientModel.ProtectedItem)protectedItemResource.Properties;
            Name = protectedItemResource.Name;
            Id = protectedItemResource.Id;
            LatestRecoveryPoint = protectedItem.LastRecoveryPoint;
        }
    }

    /// <summary>
    /// Represents Azure Backup Item ExtendedInfo Base Class
    /// </summary>
    public class ItemExtendedInfoBase : ObjectBase
    {
    }

    public class RecoveryPointBase : ItemContext
    {
        private global::Microsoft.Azure.Management.RecoveryServices.Backup.Models.RecoveryPointResource rp;

        /// <summary>
        /// 
        /// </summary>
        public string RecoveryPointId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///Type of recovery point (appConsistent\CrashConsistent etc) 
        /// </summary>
        ///
        public String RecoveryPointType { get; set; }

        /// <summary>
        /// Time of RecoveryPoint
        /// </summary>
        public DateTime RecoveryPointTime { get; set; }

        public RecoveryPointBase()
            : base()
        {
        }
    }


    public class PolicyBase : ManagementContext
    {
        public string Name { get; set; }

        public WorkloadType WorkloadType { get; set; }

        public string Id { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentException(Resources.PolicyNameIsEmptyOrNull);
            }

            if (string.IsNullOrEmpty(Id))
            {
                throw new ArgumentException(Resources.PolicyIdIsEmptyOrNull);
            }
        }
    }

    public class RetentionPolicyBase : ObjectBase
    {      
    }

    public class SchedulePolicyBase : ObjectBase
    {      
    }

    public class JobBase : ManagementContext
    {
        public string ActivityId { get; set; }

        public string JobId { get; set; }

        public string Operation { get; set; }

        public string Status { get; set; }

        public string WorkloadName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public TimeSpan Duration { get; set; }       
    }

    /// <summary>
    /// This class contains job error message details.
    /// </summary>
    public class JobErrorInfoBase
    {
        public string ErrorMessage { get; set; }

        public List<string> Recommendations { get; set; }
    }

    /// <summary>
    /// This class contains job sub tasks detail.
    /// </summary>
    public class JobSubTaskBase
    {
        public string Name { get; set; }

        public string Status { get; set; }
    }
}

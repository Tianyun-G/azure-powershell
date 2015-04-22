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

using Microsoft.Azure.Commands.Compute.Common;
using Microsoft.Azure.Commands.Compute.Models;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Compute
{
    [Cmdlet(VerbsCommon.Get, ProfileNouns.RemoteDesktopFile)]
    public class GetAzureRemoteDesktopFile : VirtualMachineBaseCmdlet
    {
        [Parameter(
           Mandatory = true,
           Position = 0,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "The resource group name.")]
        [ValidateNotNullOrEmpty]
        public override string ResourceGroupName { get; set; }

        [Alias("ResourceName", "VMName")]
        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The resource name.")]
        [ValidateNotNullOrEmpty]
        public override string Name { get; set; }

        
        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            if (!string.IsNullOrEmpty(this.Name))
            {
                if (Status)
                {
                    var result = this.VirtualMachineClient.GetWithInstanceView(this.ResourceGroupName, this.Name);
                    WriteObject(result.ToPSVirtualMachineInstanceView(this.ResourceGroupName, this.Name));
                }
                else
                {
                    var result = this.VirtualMachineClient.Get(this.ResourceGroupName, this.Name);
                    WriteObject(result.ToPSVirtualMachine(this.ResourceGroupName));
                }
            }
            else
            {
                VirtualMachineListResponse result = null;

                if (!string.IsNullOrEmpty(this.ResourceGroupName))
                {
                    result = this.VirtualMachineClient.List(this.ResourceGroupName);
                }
                else if (this.NextLink != null)
                {
                    result = this.VirtualMachineClient.ListNext(this.NextLink.ToString());
                }
                else
                {
                    var listParams = new ListParameters();
                    result = this.VirtualMachineClient.ListAll(listParams);
                }

                WriteObject(result.ToPSVirtualMachineList(this.ResourceGroupName), true);
            }
        }
    }
}

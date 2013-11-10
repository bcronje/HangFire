﻿// This file is part of HangFire.
// Copyright © 2013 Sergey Odinokov.
// 
// HangFire is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// HangFire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with HangFire.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using ServiceStack.Redis;

namespace HangFire.States
{
    public class ProcessingState : JobState
    {
        public static readonly string Name = "Processing";

        public ProcessingState(string reason, string serverName) 
            : base(reason)
        {
            ServerName = serverName;
        }

        public string ServerName { get; private set; }

        public override string StateName { get { return Name; } }

        public override IDictionary<string, string> GetProperties(JobDescriptor descriptor)
        {
            return new Dictionary<string, string>
                {
                    { "StartedAt", JobHelper.ToStringTimestamp(DateTime.UtcNow) },
                    { "ServerName", ServerName }
                };
        }

        public override void Apply(JobDescriptor descriptor, IRedisTransaction transaction)
        {
            transaction.QueueCommand(x => x.AddItemToSortedSet(
                "hangfire:processing", descriptor.JobId, JobHelper.ToTimestamp(DateTime.UtcNow)));
        }

        public class Descriptor : JobStateDescriptor
        {
            public override void Unapply(JobDescriptor descriptor, IRedisTransaction transaction)
            {
                transaction.QueueCommand(x => x.RemoveItemFromSortedSet(
                    "hangfire:processing", descriptor.JobId));
            }
        }
    }
}
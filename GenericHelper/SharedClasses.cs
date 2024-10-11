// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace GenericHelper
{
    public class RabbitNotifyMessage
    {
        public string? id { get; set; }
        public string? db { get; set; }
        public string? collection { get; set; }
    }

    public class TestObject
    {
        public string Name { get; set; }
        public string Property { get; set; }
        public string Source { get; set; }
    }

    public class RabbitIngressMessage
    {
        public string id { get; set; }
        public string provider { get; set; }
        public string timestamp { get; set; }
        //public object Rawdata { get; set; }
        public string rawdata { get; set; }
    }
}

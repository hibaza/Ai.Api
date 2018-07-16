using System;
using System.Collections.Generic;
using System.Text;
using Ai.Core.Helpers;

namespace Ai.Domain.Entities
{
    public abstract class BaseEntity
    {
        public string _id { get; set; }

        public string id
        {
            get { return _id.ToString(); }
            set { _id = value; }
        }

        public DateTime created_time { get; set; }
        public DateTime? updated_time { get; set; }
        public BaseEntity()
        {
            id = CommonHelper.GenerateNineDigitUniqueNumber();
            _id = id;
            created_time = DateTime.UtcNow;
            updated_time = created_time;
        }

    }

}

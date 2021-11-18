using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliaryLib.Helpers
{
    public class PageResponse<T>
    {
        public int PageLength { get; set; }
        public int PageNumber { get; set; }
        [JsonIgnore]
        public int Skip
        {
            get { return PageLength * (PageNumber - 1); }
        }
        [JsonIgnore]
        public int Take
        {
            get { return PageLength; }
        }
        //public int CurrentPage { get; set; }
        //public int NumerOfItemsPerPage { get; set; }
        public long TotalItems { get; set; }

        public int TotalPages
        {
            get
            {
                return TotalItems > 0 ? (int)(TotalItems / PageLength) : 0;
            }
        }
        public int ItemCount
        {
            get
            {
                return Items.Count;
            }
        }
        public ICollection<T> Items { get; set; } = new List<T>();

        public PageResponse(int? pageLength = null, int? pageNumber = null)
        {
            if (pageLength.HasValue)
                this.PageLength = pageLength.Value;
            else
                this.PageLength = 10;
            if (pageNumber.HasValue)
                this.PageNumber = pageNumber.Value;
            else
                this.PageNumber = 1;
            if (PageNumber < 1)
                PageNumber = 1;
            if (PageLength < 1)
                PageLength = 1;
        }

        public PageResponse() { }
    }
}

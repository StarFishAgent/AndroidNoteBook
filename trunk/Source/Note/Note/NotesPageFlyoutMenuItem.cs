using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note
{

    public class NotesPageFlyoutMenuItem
    {
        public NotesPageFlyoutMenuItem()
        {
            TargetType = typeof(NotesPageFlyoutMenuItem);
        }
        public long Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}
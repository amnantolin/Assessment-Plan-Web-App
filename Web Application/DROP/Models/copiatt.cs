//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DROP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class copiatt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public copiatt()
        {
            this.processes = new HashSet<process>();
        }
    
        public int copiatt_id { get; set; }
        public int pi_id { get; set; }
        public int course_id { get; set; }
        public int at_id { get; set; }
        public float target { get; set; }
    
        public virtual at at { get; set; }
        public virtual pi pi { get; set; }
        public virtual course course { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<process> processes { get; set; }
    }
}

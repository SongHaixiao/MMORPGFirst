
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace GameServer
{

using System;
    using System.Collections.Generic;
    
public partial class TCharacter
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public TCharacter()
    {

        this.MapID = 1;

        this.Items = new HashSet<TCharacterItem>();

        this.Quests = new HashSet<TCharacterQuest>();

    }


    public int ID { get; set; }

    public int TID { get; set; }

    public string Name { get; set; }

    public int Class { get; set; }

    public int MapID { get; set; }

    public int MapPosX { get; set; }

    public int MapPosY { get; set; }

    public int MapPosZ { get; set; }

    public long Gold { get; set; }

    public byte[] Equips { get; set; }



    public virtual TPlayer Player { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<TCharacterItem> Items { get; set; }

    public virtual TCharacterBag Bag { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<TCharacterQuest> Quests { get; set; }

}

}

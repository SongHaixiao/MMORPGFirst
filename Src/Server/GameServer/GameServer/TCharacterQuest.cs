
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
    
public partial class TCharacterQuest
{

    public int Id { get; set; }

    public int TCharacterID { get; set; }

    public int QuestID { get; set; }

    public int Target1 { get; set; }

    public int Target2 { get; set; }

    public int Target3 { get; set; }

    public int Status { get; set; }



    public virtual TCharacter Owner { get; set; }

}

}

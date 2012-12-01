using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LerosClient.Models
{
    
    [DotLiquid.LiquidType("Objects")]
    public class Result : Drop
    {
        public Session[] objects ;
        
       
        
    }
}

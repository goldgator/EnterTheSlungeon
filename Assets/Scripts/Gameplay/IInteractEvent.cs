using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


interface IInteractEvent
{
    public void OnInteract();
    public void OnInteractEnter();
    public void OnInteractLeave();
}


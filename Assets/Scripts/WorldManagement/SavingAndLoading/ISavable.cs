using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving{
public interface ISavable
{
    public object save();
    public void load();

}
}
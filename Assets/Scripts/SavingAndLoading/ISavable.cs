using System.Collections;
using System.Collections.Generic;

namespace Saving{
public interface ISavable
{
    public object save();
    public void load();

}
}
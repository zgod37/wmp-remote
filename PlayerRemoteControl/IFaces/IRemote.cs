using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerRemoteControl.IFaces {
    public interface IRemote {

        void registerObserver(IPlayer observer);
        void unregisterObserver(IPlayer observer);
    }
}

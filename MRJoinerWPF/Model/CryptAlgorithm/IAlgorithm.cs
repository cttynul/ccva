using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRJoinerWPF.Model.CryptAlgorithm
{
    public interface IAlgorithm
    {
        byte[] encrypt(byte[] file, string password);
        byte[] decrypt(byte[] file, string password);
    }
}

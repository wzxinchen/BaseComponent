using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.PrivilegeManagement.Provider
{
    public interface IPrivilegeProvider
    {

        bool Exist(int id, string name);

        void Add(DTO.Privilege privilege);

        bool Exist(int id);

        bool Exist(string name);

        DTO.Privilege Get(int id);

        void Update(DTO.Privilege privilege);

        bool HasUser();
    }
}

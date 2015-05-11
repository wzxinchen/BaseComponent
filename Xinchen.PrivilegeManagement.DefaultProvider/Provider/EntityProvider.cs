namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xinchen.DynamicObject;
    using Xinchen.ObjectMapper;
    using Xinchen.PrivilegeManagement.DefaultProvider;
    using Xinchen.PrivilegeManagement.Provider;
    using Xinchen.Utils.Entity;

    public class EntityProvider<TAppModel, TDbEntity> : IEntityProvider<TAppModel>
    {
        private ProviderContext _context;
        private static EntityProvider<TAppModel, TDbEntity> _instance;
        private static object _syncLock;

        static EntityProvider()
        {
            EntityProvider<TAppModel, TDbEntity>._syncLock = new object();
        }

        private EntityProvider()
        {
            this._context = ProviderContext.GetInstance();
        }

        public TAppModel Add(TAppModel entity)
        {
            TDbEntity local = EntityMapper.Map<TAppModel, TDbEntity>(entity);
            this._context.EntitySet<TDbEntity>().Create(local);
            return entity;
        }

        public bool Exist(int id)
        {
            return this._context.EntitySet<TDbEntity>().Exists(id);
        }

        public TAppModel Get(Action<TAppModel> condition)
        {
            TAppModel local = DynamicProxy.CreateDynamicProxy<TAppModel>();
            condition(local);
            Dictionary<string, object> modifiedProperties = DynamicProxy.GetModifiedProperties(local);
            TDbEntity source = this._context.EntitySet<TDbEntity>().GetList(modifiedProperties).FirstOrDefault<TDbEntity>();
            if (source == null)
            {
                return default(TAppModel);
            }
            return EntityMapper.Map<TDbEntity, TAppModel>(source);
        }

        public TAppModel Get(int id)
        {
            TDbEntity byId = this._context.EntitySet<TDbEntity>().GetSingle(id);
            if (byId == null)
            {
                return default(TAppModel);
            }
            return EntityMapper.Map<TDbEntity, TAppModel>(byId);
        }

        public static EntityProvider<TAppModel, TDbEntity> GetInstance()
        {
            if (EntityProvider<TAppModel, TDbEntity>._instance == null)
            {
                lock (EntityProvider<TAppModel, TDbEntity>._syncLock)
                {
                    if (EntityProvider<TAppModel, TDbEntity>._instance == null)
                    {
                        EntityProvider<TAppModel, TDbEntity>._instance = new EntityProvider<TAppModel, TDbEntity>();
                    }
                }
            }
            return EntityProvider<TAppModel, TDbEntity>._instance;
        }

        public IList<TAppModel> GetList(Action<TAppModel> condition)
        {
            TAppModel local = DynamicProxy.CreateDynamicProxy<TAppModel>();
            condition(local);
            Dictionary<string, object> modifiedProperties = DynamicProxy.GetModifiedProperties(local);
            IList<TDbEntity> by = this._context.EntitySet<TDbEntity>().GetList(modifiedProperties);
            List<TAppModel> list2 = new List<TAppModel>();
            foreach (TDbEntity local2 in by)
            {
                list2.Add(EntityMapper.Map<TDbEntity, TAppModel>(local2));
            }
            return list2;
        }

        public int GetUniqueId()
        {
            return this._context.EntitySet<TDbEntity>().GetSequenceId();
        }

        public bool IsEmpty()
        {
            return this._context.EntitySet<TDbEntity>().IsEmpty();
        }

        public void Remove(TAppModel condition)
        {
            throw new NotImplementedException();
        }

        public void Remove(Action<TAppModel> condition)
        {
            TAppModel local = DynamicProxy.CreateDynamicProxy<TAppModel>();
            condition(local);
            this._context.EntitySet<TDbEntity>().DeleteBy(DynamicProxy.GetModifiedProperties(local));
        }

        public void Update(TAppModel entity)
        {
            TDbEntity target = DynamicProxy.CreateDynamicProxy<TDbEntity>();
            EntityMapper.Map<TAppModel, TDbEntity>(entity, target);
            this._context.EntitySet<TDbEntity>().Update(target);
        }
    }
}


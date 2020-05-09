using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Context;
using Framework.Utils.Results;
using Framework.EntityFramework.Utils;
using Framework.DataAccess.Contract;

namespace Framework.EntityFramework.Repositories
{
    public abstract class EntityFrameworkRepository<T, D> : IRepositoryBase<T>
            where T : class
            where D : DbContextBase, new()
    {
        #region ASYNC

        public virtual async Task<ExecutionResult> DeleteAsync(int pId)
        {
            using (D db = new D())
            {
                T item = await db.Set<T>().FindAsync(pId);
                if (item != null)
                {
                    try
                    {
                        db.Set<T>().Remove(item);
                        await db.SaveChangesAsync();
                        return CreateDDBBResult(EExecutionResultCode.OK);
                    }
                    catch (Exception ex)
                    {
                        return CreateDDBBResult(EExecutionResultCode.DeleteError, ex.Message);
                    }
                }
                else
                {
                    throw new Exception(Validaciones.DAODeleteItemNotExists);
                }
            }
        }

        public virtual async Task<List<T>> FilterAsync(Func<T, bool> pFilter)
        {
            using (D db = new D())
            {
                try
                {
                    return await Task.FromResult(db.Set<T>().Where(pFilter).ToList());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            using (D db = new D())
            {
                try
                {
                    return await db.Set<T>().ToListAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public virtual async Task<T> GetByIdAsync(int pId)
        {
            using (D db = new D())
            {
                try
                {
                    return await db.Set<T>().FindAsync(pId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public virtual async Task<ExecutionResult> AddAsync(T pItem)
        {
            using (D db = new D())
            {
                try
                {
                    db.Set<T>().Add(pItem);
                    await db.SaveChangesAsync();
                    return CreateDDBBResult(EExecutionResultCode.OK);
                }
                catch (Exception ex)
                {
                    return CreateDDBBResult(EExecutionResultCode.AddOrUpdateError, ex.Message);
                }
            }
        }

        public virtual async Task<ExecutionResult> UpdateAsync(T pItem, int pId)
        {
            using (D db = new D())
            {
                T item = await db.Set<T>().FindAsync(pId);
                if (item != null)
                {
                    try
                    {
                        db.Entry(item).CurrentValues.SetValues(pItem);
                        await db.SaveChangesAsync();
                        return CreateDDBBResult(EExecutionResultCode.OK);
                    }
                    catch (Exception ex)
                    {
                        return CreateDDBBResult(EExecutionResultCode.AddOrUpdateError, ex.Message);
                    }
                }
                else
                {
                    throw new Exception(Validaciones.DAOUpdateItemNotExists);
                }
            }
        }

        public virtual async Task<ExecutionResult> AddBulkAsync(List<T> pItems)
        {
            try
            {
                using (D db = new D())
                {
                    db.Set<T>().AddRange(pItems);
                    await db.SaveChangesAsync();
                    return CreateDDBBResult(EExecutionResultCode.OK);
                }
            }
            catch (Exception ex)
            {
                return CreateDDBBResult(EExecutionResultCode.BulkInsertError, ex.Message);
            }
        }

        #endregion
        
        #region SYNC

        public virtual ExecutionResult Update(T pItem, int pId)
        {
            using (D db = new D())
            {
                T item = db.Set<T>().Find(pId);
                if (item != null)
                {
                    try
                    {
                        db.Entry(item).CurrentValues.SetValues(pItem);
                        db.SaveChanges();
                        return CreateDDBBResult(EExecutionResultCode.OK);
                    }
                    catch (Exception ex)
                    {
                        return CreateDDBBResult(EExecutionResultCode.AddOrUpdateError, ex.Message);
                    }
                }
                else
                {
                    throw new Exception(Validaciones.DAOUpdateItemNotExists);
                }
            }
        }

        public virtual ExecutionResult Add(T pItem)
        {
            try
            {
                using (D db = new D())
                {
                    db.Set<T>().Add(pItem);
                    db.SaveChanges();
                    return CreateDDBBResult(EExecutionResultCode.OK);
                }
            }
            catch (Exception ex)
            {
                return CreateDDBBResult(EExecutionResultCode.AddOrUpdateError, ex.Message);
            }
        }

        public virtual ExecutionResult AddBulk(List<T> pItems)
        {
            try
            {
                using (D db = new D())
                {
                    db.Set<T>().AddRange(pItems);
                    db.SaveChanges();
                    return CreateDDBBResult(EExecutionResultCode.OK);
                }
            }
            catch (Exception ex)
            {
                return CreateDDBBResult(EExecutionResultCode.BulkInsertError, ex.Message);
            }
        }

        public virtual ExecutionResult Delete(int pId)
        {
            using (D db = new D())
            {
                T item = db.Set<T>().Find(pId);
                if (item != null)
                {
                    try
                    {
                        db.Set<T>().Remove(item);
                        db.SaveChanges();
                        return CreateDDBBResult(EExecutionResultCode.OK);
                    }
                    catch (Exception ex)
                    {
                        return CreateDDBBResult(EExecutionResultCode.DeleteError, ex.Message);
                    }
                }
                else
                {
                    throw new Exception(Validaciones.DAODeleteItemNotExists);
                }
            }
        }

        public virtual List<T> GetAll()
        {
            try
            {
                using (D db = new D())
                {
                    return db.Set<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual List<T> Filter(Func<T, bool> pFilter)
        {
            try
            {
                using (D db = new D())
                {
                    return db.Set<T>().Where(pFilter).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual T GetById(int pId)
        {
            try
            {
                using (D db = new D())
                {
                    return db.Set<T>().Find(pId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ExecutionResult CreateDDBBResult(EExecutionResultCode pCode, string pErrorMsg = "")
        {
            List<string> errores = new List<string>();
            if (string.IsNullOrEmpty(pErrorMsg) == false)
                errores.Add(pErrorMsg);

            return new ExecutionResult() { ErrorMsg = errores, ResultCode = pCode };
        }

        public ExecutionResult CreateDDBBResult(EExecutionResultCode pCode, List<string> pErrorMsg)
        {
            return new ExecutionResult() { ErrorMsg = pErrorMsg, ResultCode = pCode };
        }

        #endregion
    }
}

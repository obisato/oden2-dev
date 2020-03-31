﻿using Common.Tools.Database;
using System;

namespace Common.Services
{
    public class AuthentificationService : IService
    {
        private readonly IDbAccessor dbAccessor;
        public AuthentificationService(IDbAccessor dbAccessor)
        {
            this.dbAccessor = dbAccessor ?? throw new ArgumentNullException(nameof(dbAccessor));
        }
        public void Dispose()
        {
            dbAccessor.Dispose();
        }

        public ServiceResults DoService(string[] inputData)
        {
            var output = new ServiceResults();
            output.Result.Add("MESSAGE", string.Empty);

            if (inputData.Length == 3)
            {
                if (!Equals(inputData[1], inputData[2]))
                {
                    output.Result["MESSAGE"] = Messages.EM05;
                    return output;
                }
                else
                {
                    try
                    {
                        dbAccessor.ActivateAccount(inputData[0]);
                        output.Result["MESSAGE"] = Messages.PM01;
                        output.IsSuccessed = true;
                        return output;
                    }
                    catch
                    {
                        output.Result["MESSAGE"] = Messages.EM01;
                        return output;
                    }
                }
            }
            output.Result["MESSAGE"] = Messages.EM10;
            return output;
        }
    }
}

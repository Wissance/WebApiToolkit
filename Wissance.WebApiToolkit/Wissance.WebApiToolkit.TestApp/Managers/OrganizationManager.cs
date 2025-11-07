using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Ef.Managers;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.TestApp.Factories;

namespace Wissance.WebApiToolkit.TestApp.Managers
{
    public class OrganizationManager : EfModelManager<ModelContext, OrganizationDto, OrganizationEntity, int>
    {
        public OrganizationManager(ModelContext dbContext, Func<OrganizationEntity, IDictionary<string, string>, bool> filterFunc, Func<OrganizationEntity, OrganizationDto> createFunc, ILoggerFactory loggerFactory) 
            : base(dbContext, filterFunc, createFunc, null, null, loggerFactory)
        {
            _dbContext = dbContext;
        }

        public override async Task<OperationResultDto<OrganizationDto>> CreateAsync(OrganizationDto data)
        {
            try
            {
                OrganizationEntity organization = new OrganizationEntity()
                {
                    Name = data.Name,
                    ShortName = data.ShortName,
                    TaxNumber = data.TaxNumber
                };
                
                // todo(UMV): temporarily offed
                /*if (data.Codes != null)
                {
                    foreach (int code in data.Codes)
                    {
                        CodeEntity organizationCode = await _dbContext.Codes.FirstOrDefaultAsync(c => c.Id == code);
                        organization.Codes.Add(organizationCode);
                    }
                }*/

                int result = await _dbContext.SaveChangesAsync();
                if (result < 0)
                {
                    return new OperationResultDto<OrganizationDto>(false, (int) HttpStatusCode.InternalServerError,
                        $"An unknown error occurred during Organization create", null);
                }

                return new OperationResultDto<OrganizationDto>(true, (int) HttpStatusCode.Created, String.Empty,
                    OrganizationFactory.Create(organization));
            }
            catch (Exception e)
            {
                return new OperationResultDto<OrganizationDto>(false, (int) HttpStatusCode.InternalServerError,
                    $"An error occurred during Organization create: {e.Message}", null);
            }
        }

        public override async Task<OperationResultDto<OrganizationDto>> UpdateAsync(int id, OrganizationDto data)
        {
            try
            {
                OrganizationEntity organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == id);
                if (organization == null)
                {
                    return new OperationResultDto<OrganizationDto>(false, (int) HttpStatusCode.NotFound,
                        $"Organization with id: {id} was not found", null);
                }

                organization.Name = data.Name;
                organization.ShortName = data.ShortName;
                organization.TaxNumber = data.TaxNumber;

                int result = await _dbContext.SaveChangesAsync();
                if (result < 0)
                {
                    return new OperationResultDto<OrganizationDto>(false, (int) HttpStatusCode.InternalServerError,
                        $"An unknown error occurred during Organization update", null);
                }
                return new OperationResultDto<OrganizationDto>(true, (int) HttpStatusCode.OK,
                    String.Empty, OrganizationFactory.Create(organization));
            }
            catch (Exception e)
            {
                return new OperationResultDto<OrganizationDto>(false, (int) HttpStatusCode.InternalServerError,
                    $"An error occurred during Organization update: {e.Message}", null);
            }
        }

        private readonly ModelContext _dbContext;
    }
}
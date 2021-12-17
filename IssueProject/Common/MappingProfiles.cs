using AutoMapper;
using IssueProject.Entity;
using IssueProject.Models.Department;
using IssueProject.Models.Issue;
using IssueProject.Models.IssueActivity;
using IssueProject.Models.IssueActivityDetail;
using IssueProject.Models.IssueAttachment;
using IssueProject.Models.IssueNote;
using IssueProject.Models.IssueRelevantDepartMent;
using IssueProject.Models.IssueRole;
using IssueProject.Models.Precondition;
using IssueProject.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Common
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Issue, IssueInfo>()
                .ForPath(x => x.IssueActivitiyInfos, opt => opt.MapFrom(src => src.IssueActivitiys))
                .ForPath(x => x.IssueAttachmentInfos, opt => opt.MapFrom(src => src.IssueAttachments))
                .ForPath(x => x.IssueNoteInfos, opt => opt.MapFrom(src => src.IssueNotes))
                .ForPath(x => x.IssuePreconditionInfos, opt => opt.MapFrom(src => src.IssuePreconditions))
                .ForPath(x => x.IssueRelevantDepartmentInfos, opt => opt.MapFrom(src => src.IssueRelevantDepartmants))
                .ForPath(x => x.IssueRoleInfos, opt => opt.MapFrom(src => src.IssueRoles));
            CreateMap<IssueActivitiy, IssueActivityInfo>()
                 .ForPath(x => x.IssueActivityDetailInfos, opt => opt.MapFrom(src => src.IssueActivitiyDetails));
            CreateMap<IssueActivitiyDetail, IssueActivitiyDetailInfo>()
                 .ForPath(x => x.IssueActivityDetailInfos, opt => opt.MapFrom(src => src.IssueActivitiyDetails));
            CreateMap<IssueAttachment, IssueAttachmentInfo>();
            CreateMap<IssueNote, IssueNoteInfo>();
            CreateMap<IssuePrecondition, IssuePreconditionInfo>();
            CreateMap<IssueRelevantDepartmant, IssueRelevantDepartmentInfo>();
            CreateMap<IssueRole, IssueRoleInfo>();
            CreateMap<Department, DepartmentInfo>();
            CreateMap<Role, RoleInfo>();
          
        }
    }
}

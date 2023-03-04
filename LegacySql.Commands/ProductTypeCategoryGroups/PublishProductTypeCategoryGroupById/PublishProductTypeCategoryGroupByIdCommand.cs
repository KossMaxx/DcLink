using MediatR;

namespace LegacySql.Commands.ProductTypeCategoryGroups.PublishProductTypeCategoryGroupById
{
    public class PublishProductTypeCategoryGroupByIdCommand : IRequest
    {
        public PublishProductTypeCategoryGroupByIdCommand(int groupId)
        {
            GroupId = groupId;
        }

        public int GroupId { get; }
    }
}

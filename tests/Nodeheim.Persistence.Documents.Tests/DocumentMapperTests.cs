using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents.Tests;

public class DocumentMapperTests
{
    [Theory]
    [InlineData(" 3f8e2c1a-0b7d-4e55-9a61-2d4c8b0f7e13")]
    [InlineData("3f8e2c1a-0b7d-4e55-9a61-2d4c8b0f7e13\t")]
    [InlineData(" 3f8e2c1a-0b7d-4e55-9a61-2d4c8b0f7e1")]
    public void Read_WithWhitespaceInStateId_RejectsDocument(string stateId)
    {
        var root = new CompoundValue(
        [
            new("nodeheimFormat", new TextValue("0.1")),
            new("stateId", new TextValue(stateId)),
        ]);
        var issues = new IssueCollector();

        Document? document = DocumentMapper.Read(root, issues);

        Assert.Null(document);
        Issue issue = Assert.Single(issues.Issues);
        Assert.Equal("format.invalidStructure", issue.Code);
        Assert.Equal("/stateId", issue.Path);
    }
}

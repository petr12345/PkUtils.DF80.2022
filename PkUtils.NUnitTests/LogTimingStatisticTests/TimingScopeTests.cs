// Ignore Spelling: Utils
//
using PK.PkUtils.LogTimingStatistic;
namespace PK.PkUtils.NUnitTests.LogTimingStatisticTests;

#pragma warning disable IDE0079  // Remove unnecessary suppression
#pragma warning disable CA1859    // Change type of variable ...

[TestFixture()]
public class TimingScopeTests
{
    private const string _topicName1 = "topicName1";
    private const string _topicName2 = "topicName2";

    [Test()]
    public void TimingScopeConstructorTest()
    {
        // Arrange
        // Act            
        ITimingScope scope = new TimingScope();

        // Assert
        Assert.That(scope.GetAllTopics(), Is.Empty);
    }

    [Test()]
    public void TryGetTopicTest()
    {
        // Arrange
        ITimingScope scope = new TimingScope();

        // Act            
        scope.AddOccurrence(_topicName1, TimeSpan.FromSeconds(2));
        scope.AddOccurrence(_topicName2, TimeSpan.FromSeconds(4));
        ITimingTopic topic1 = scope.TryGetTopic(_topicName1);
        ITimingTopic topic2 = scope.TryGetTopic(_topicName2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(topic1, Is.Not.Null);
            Assert.That(topic2, Is.Not.Null);
        }
    }


    [Test()]
    public void AddOccurrenceTest()
    {
        // Arrange
        ITimingScope scope = new TimingScope();

        // Act            
        var topic1 = scope.AddOccurrence(_topicName1, TimeSpan.FromSeconds(22));
        var topic2 = scope.AddOccurrence(_topicName2, TimeSpan.FromSeconds(44));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(topic1, Is.Not.Null);
            Assert.That(topic2, Is.Not.Null);
        }
    }

    [Test()]
    public void GetAllTopicsTest()
    {
        // Arrange
        ITimingScope scope = new TimingScope();

        // Act            
        ITimingTopic topic1 = scope.AddOccurrence(_topicName1, TimeSpan.FromSeconds(222));
        ITimingTopic topic2 = scope.AddOccurrence(_topicName2, TimeSpan.FromSeconds(444));
        IReadOnlyList<ITimingTopic> all = scope.GetAllTopics();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(all, Contains.Item(topic1));
            Assert.That(all, Contains.Item(topic2));
            Assert.That(all, Has.Count.EqualTo(2));
        }
    }

    [Test()]
    public void ClearTest()
    {
        // Arrange
        ITimingScope scope = new TimingScope();

        // Act            
        scope.AddOccurrence(_topicName1, TimeSpan.FromSeconds(22));
        scope.AddOccurrence(_topicName2, TimeSpan.FromSeconds(44));
        scope.Clear();

        // Assert
        Assert.That(scope.GetAllTopics(), Is.Empty);
    }
}
#pragma warning restore CA1859    // Change type of variable ...
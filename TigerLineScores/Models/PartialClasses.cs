using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TigerLineScores.Models
{
    [MetadataType(typeof(CompMainMetadata))]
    public partial class CompMain
    {
    }

    [MetadataType(typeof(CourseDetailMetadata))]
    public partial class CourseDetail
    {
    }

    [MetadataType(typeof(CourseMainMetadata))]
    public partial class CourseMain
    {
    }

    [MetadataType(typeof(ProfileMetadata))]
    public partial class Profile
    {
    }

    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
    }

    [MetadataType(typeof(ScoreCardImageMetadata))]
    public partial class ScoreCardImage
    {
    }

    [MetadataType(typeof(CompScoreMetadata))]
    public partial class CompScore
    {
    }

    [MetadataType(typeof(UpcomingRndMetadata))]
    public partial class UpcomingRnd
    {
    }
}
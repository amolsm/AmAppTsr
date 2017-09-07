
  function FillCourse() {
      var CategoryId = $('#CategoryId').val();
      $.ajax({
          url: '/Application/FillCourse',
          type: "GET",
          dataType: "JSON",
          data: { CategoryId: CategoryId },
          success: function (Courses) {
              $("#CourseId").html(""); // clear before appending new list
              $("#CourseId").append(
                      $('<option></option>').val("").html(""));
              $.each(Courses, function (i, c) {
                  $("#CourseId").append(
                      $('<option></option>').val(c.CourseId).html(c.CourseName));
              });
          }
      });
  }

function FillBatch() {
    var CourseId = $('#CourseId').val();
    $.ajax({
        url: '/Application/FillBatch',
        type: "GET",
        dataType: "JSON",
        data: { CourseId: CourseId },
        success: function (Courses) {
            $("#BatchId").html(""); // clear before appending new list
            $("#BatchId").append(
                    $('<option></option>').val("").html(""));
            $.each(Courses, function (i, c) {
                $("#BatchId").append(
                    $('<option></option>').val(c.BatchId).html(c.BatchCode));
            });
        }
    });
}

 

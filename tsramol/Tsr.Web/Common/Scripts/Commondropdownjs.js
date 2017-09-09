
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
      debugger;
    var CourseId = $('#CourseId').val();
    $.ajax({
        url: '/Application/FillBatchAll',
        type: "GET",
        dataType: "JSON",
        data: { CourseId: CourseId },
        success: function (Batches) {
            $("#BatchId").html(""); // clear before appending new list
            $("#BatchId").append(
                    $('<option></option>').val("").html(""));
            $.each(Batches, function (i, c) {
                $("#BatchId").append(
                    $('<option></option>').val(c.BatchId).html(GetDate(c.StartDate)));
            });
        }
    });
  }

  function GetDate(value) {
      
      var today = new Date(parseInt(value.substr(6)))
      var dd = today.getDate();
      var mm = today.getMonth() + 1; //January is 0!

      var yyyy = today.getFullYear();
      if (dd < 10) {
          dd = '0' + dd;
      }
      if (mm < 10) {
          mm = '0' + mm;
      }
      var today = dd + '/' + mm + '/' + yyyy;
      return today
  }

 

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
                $('<option></option>').val(0).html("Select Course"));

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
        url: '/Application/FillBatchAll',
        type: "GET",
        dataType: "JSON",
        data: { CourseId: CourseId },
        success: function (Batches) {
            $("#BatchId").html(""); // clear before appending new list
            $("#BatchId").append(
                $('<option></option>').val(0).html("Select Batch"));

            $.each(Batches, function (i, c) {
                $("#BatchId").append(
                    $('<option></option>').val(c.BatchId).html(c.Name));
            });
        }
    });
}
function FillStudents() {
    var BatchId = $('#BatchId').val();
    $.ajax({
        url: '/Fees/FillStudentsBatchwise',
        type: "GET",
        dataType: "JSON",
        data: { BatchId: BatchId },
        success: function (Students) {
            $("#StudentId").html(""); // clear before appending new list
            $("#StudentId").append(
                $('<option></option>').val(0).html("Select Student"));

            $.each(Students, function (i, c) {
                $("#StudentId").append(
                    $('<option></option>').val(c.ApplicationId).html(c.Name));
            });
        }
    });
}
function FillStudentsP() {
    var PackageId = $('#PackageId').val();
    $.ajax({
        url: '/Fees/FillStudentsForPackageScrutinee',
        type: "GET",
        dataType: "JSON",
        data: { PackageId: PackageId },
        success: function (Students) {
            $("#StudentIdP").html(""); // clear before appending new list
            $("#StudentIdP").append(
                $('<option></option>').val(0).html("Select Student"));

            $.each(Students, function (i, c) {
                $("#StudentIdP").append(
                    $('<option></option>').val(c.ApplicationId).html(c.Name));
            });
            $("#StudentIdP").chosen({
                //disable_search_threshold: 10,
                no_results_text: "Oops, nothing found!",
                width: "95%"
            });
        }
    });
}
function ShowDiv() {
    var PaymentMode = $('#PaymentMode').val();
    if (PaymentMode == "Cheque") {

        $("#draft").hide();
        $("#cheq").show();
        $("#bnk").show();
    }
    else if (PaymentMode == "DD") {
        $("#cheq").hide();
        $("#draft").show();
        $("#bnk").show();
    }
    else
    {
        $("#cheq").hide();
        $("#draft").hide();
        $("#bnk").hide();
    }
}
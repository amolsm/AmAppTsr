
$(function () {
    $('.date').datepicker({ format: 'dd-MM-yyyy' });
        $('.edit-mode').hide();  
        $('.edit-item').on('click', function () {  
            $('.edit-mode').hide();  
            $('.delete-mode').hide();  
            $('.display-mode').show();  
            var tr = $(this).parents('tr:first');  
            tr.find('.edit-mode, .display-mode').toggle();  
        });  
        $('.cancel-item').on('click', function () {  
            var tr = $(this).parents('tr:first');  
            tr.find('.display-mode,.edit-mode').toggle();  
        });  
        //$('.delete-item').on('click', function () {  
        //    if (confirm("Are you sure to delete this contact?")) {  
        //        var tr = $(this).parents('tr:first');  
        //        var ID = $(this).prop('id');  
        //        //Deletes the record with ID sent below  
        //        $.post(  
        //            '/Home/DeleteContact/',  
        //            { ID: ID },  
        //            function (item) {  
        //                tr.remove();  
        //            }, "json");  
        //        location.reload();  
        //    }  
        //});  
        $('.save-item').on('click', function () {  
            $('#pro').show();
         
            var tr = $(this).parents('tr:first');  
            var ApplicationId = $(this).prop('id');
            var StudentId = tr.find('[name=StudentId-Edit]').val();
            var Name = tr.find('[name=Name-Edit]').val();
            var DOB = tr.find('[name=DOB-Edit]').val();
            var Cdcno = tr.find('[name=Cdcno-Edit]').val();
            var PassportNo = tr.find('[name=PassportNo-Edit]').val();
            var Rank = tr.find('[name=Rank-Edit]').val();
            var Grade = tr.find('[name=Grade-Edit]').val();
            var IndosNo = tr.find('[name=IndosNo-Edit]').val();
            $.ajax({  
                type: "POST",  
                url: "/Certification/CheckListEdit/",
                data: { ApplicationId: ApplicationId, Name: Name, DOB: DOB, Cdcno: Cdcno, PassportNo: PassportNo, Rank: Rank, Grade: Grade,IndosNo:IndosNo },
                success: function (item) {  
                  if (item != null) {  
                     debugger;
                        $('#pro').hide();
                        alert("Record Update Successfully");
                        var id = tr.find('span').attr('id');
                        id.find('#StudentId').text(item.ApplicationCode);
                        id.find('#Name').text(item.FullName);
                        id.find('#DOB').text(item.DateOfBirth);
                        id.find('#Cdcno').text(item.CdcNo);
                        id.find('#PassportNo').text(item.PassportNo);
                        id.find('#Rank').text(item.RankOfCandidate);
                        id.find('#Grade').text(item.GradeOfCompetencyNo);
                        id.find('#IndosNo').text(item.InDosNo);
                        
                    }  
                    else  
                      alert('Error!');
                      
                },  
                error: function (result) {  
                    alert('Error!');  
                }  
                 
            });  
            tr.find('.edit-mode, .display-mode').toggle();  
             
        });  
    })  
  

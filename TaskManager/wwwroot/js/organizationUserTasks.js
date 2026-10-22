var userDataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    userDataTable = $('#userTblData').DataTable({
        ajax: '/user/taskItem/getAllTasksAssignedToUserInOrganization',     //This is the same (controller-action) URL we would search on the webpage
        pageLength: 10, 
        columns: [
            { data: 'title'},      //Must match a property name of the product class
            { data: 'description' },
            { data: 'dueDate' },
            { data: 'status' },
            {
                data: 'id', "width": "10%", "render": function (data) {
                    return `<div class="d-flex gap-2 justify-content-end">
                                <a href = "/user/taskItem/update?id=${data}" class="btn btn-sm btn-outline-success">
                                    <i class="bi bi-pencil-square"></i> Update
                                </a>
                                <form action = "/user/taskItem/delete?taskId=${data}" method = "post"> 
                                    <button type = "submit" class="btn btn-sm btn-outline-danger">
                                        <i class="bi bi-trash"></i> Delete
                                    </button>
                                </form>
                            
                            </div >   `;
                }
            }

        ]
    });


}


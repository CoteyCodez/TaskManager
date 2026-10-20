var userDataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    userDataTable = $('#userTblData').DataTable({
        ajax: '/user/taskItem/getAllOrgTasks',     //This is the same (controller-action) URL we would search on the webpage
        columns: [
            { data: 'title'},      //Must match a property name of the product class
            { data: 'description'},
            { data: 'status'},

        ]
    });


}


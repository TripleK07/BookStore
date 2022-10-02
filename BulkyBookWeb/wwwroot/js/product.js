var dataTable;

$(document).ready(function () {
    dataTable = $('#tblProduct').DataTable({
        ajax: {
            url: '/admin/product/getallproducts',
        },
        columns: [
            { data: 'title', width: '25%' },
            { data: 'isbn', width: '15%' },
            { data: 'price', width: '15%' },
            { data: 'author', width: '15%' },
            { data: 'category.name', width: '15%' },
            {
                data: 'id', width: '15%', render: function (data) {
                    return `
                        <a class="btn btn-warning btn-sm" href="/admin/product/upsert/${data}">
                            <i class="bi bi-pencil-square"></i> &nbsp; Edit
                        </a>
                        &nbsp;
                        <a class="btn btn-danger btn-sm" onClick=Delete('/admin/product/delete/${data}')>
                            <i class="bi bi-x-circle"></i> &nbsp; Delete
                        </a>

                    `;
                }
            }
        ]
    });
});

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
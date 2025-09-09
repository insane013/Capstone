// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

const param = $.param;

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".toggle-details").forEach(button => {
        button.addEventListener("click", function () {
            const taskId = this.getAttribute("data-task-id");
            const detailDiv = document.getElementById(`details-${taskId}`);
            if (detailDiv) {
                detailDiv.classList.toggle("d-none");
            }
        });
    });
});

// Priority modal code
document.addEventListener('DOMContentLoaded', function () {
	var modal = document.getElementById('priorityModal');
	if (!modal) return;

	modal.addEventListener('show.bs.modal', function (event) {
		var button = event.relatedTarget;
		if (!button) return;

		var taskId = button.getAttribute('data-task-id');
		var listId = button.getAttribute('data-list-id');
		var priority = button.getAttribute('data-priority');

		document.getElementById('modalTaskId').value = taskId;
		document.getElementById('modalListId').value = listId;
		document.getElementById('modalPrioritySelect').value = priority;
	});
});

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('modal-form');
    const select = document.getElementById('modalPrioritySelect');
    const saveButton = form.querySelector('button[type="submit"]');
    let initialPriority = null;

    // При открытии модалки заполняем начальные значения
    const modal = document.getElementById('priorityModal');
    modal.addEventListener('show.bs.modal', (event) => {
        const triggerButton = event.relatedTarget;
        initialPriority = triggerButton.getAttribute('data-priority');

        document.getElementById('modalTaskId').value = triggerButton.getAttribute('data-task-id');
        document.getElementById('modalTodoId').value = triggerButton.getAttribute('data-list-id');
        select.value = initialPriority;

        saveButton.disabled = true;
        saveButton.classList.add('disabled');
    });

    // Слушаем изменение select-а
    select.addEventListener('input', () => {
        const isChanged = select.value !== initialPriority;
        saveButton.disabled = !isChanged;
        saveButton.classList.toggle('disabled', !isChanged);
    });

    form.addEventListener('submit', (e) => {
        if (select.value === initialPriority) {
            e.preventDefault(); // Защита от отправки если вдруг кнопка была разблокирована случайно
        }
    });
});

// Reassign user
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".reassign-btn").forEach(function (button) {
        button.addEventListener("click", function () {
            const taskId = button.getAttribute("data-task-id");
            const todoListId = button.getAttribute("data-todo-list-id");
            const userId = button.getAttribute("data-user-id");

            const params = new URLSearchParams({
                taskId: taskId,
                todoListId: todoListId,
                currentUserId: userId
            });

            fetch('/ComponentLoader/ReAssignUser?' + params.toString(), {
                method: 'GET'
            })
                .then(response => {
                    if (!response.ok) throw new Error("Network response was not ok");
                    return response.text();
                })
                .then(html => {
                    document.getElementById("reassign-container").innerHTML = html;
                    setupReassignModal(); // инициализация после вставки формы
                })
                .catch(error => {
                    console.error("Error:", error);
                });
        });
    });
});

function setupReassignModal() {
    const reassignForm = document.querySelector('#reassignUserForm');
    if (!reassignForm) return;

    const userSelect = reassignForm.querySelector('#reassignUserSelect');
    const saveButton = reassignForm.querySelector('#reassignUserSubmit');
    const reassignModal = document.getElementById('reassignModal');
    let initialUserId = null;

    reassignModal.addEventListener('show.bs.modal', (event) => {
        const trigger = event.relatedTarget;
        initialUserId = trigger.getAttribute('data-current-user');

        const taskId = trigger.getAttribute('data-task-id');
        reassignForm.querySelector('input[name="TodoId"]').value = taskId;
        userSelect.value = initialUserId;

        saveButton.disabled = true;
        saveButton.classList.add('disabled');
    });

    userSelect.addEventListener('input', () => {
        const changed = userSelect.value !== initialUserId;
        saveButton.disabled = !changed;
        saveButton.classList.toggle('disabled', !changed);
    });

    reassignForm.addEventListener('submit', (e) => {
        if (userSelect.value === initialUserId) {
            e.preventDefault();
        }
    });
}

// Edit comment modal

$(document).on('click', '.edit-comment-btn', function () {
    const commentId = $(this).data('id');
    const commentContent = $(this).data('content');
    const commentTask = $(this).data('task');
    const commentUser = $(this).data('user');

    $('#edit-comment-id').val(commentId);
    $('#edit-comment-content').val(commentContent);
    $('#edit-comment-taskId').val(commentTask);
    $('#edit-comment-userId').val(commentUser);

    const modal = new bootstrap.Modal(document.getElementById('editCommentModal'));
    modal.show();
});

// delete comment modal

$(document).on('click', '.delete-comment-btn', function () {
    const commentId = $(this).data('id');
    const commentContent = $(this).data('content');
    const commentTask = $(this).data('task');
    const commentUser = $(this).data('user');

    $('#delete-comment-id').val(commentId);
    $('#delete-comment-taskId').val(commentTask);
    $('#delete-comment-userId').val(commentUser);
    $('#delete-comment-content').text(commentContent);

    const modal = new bootstrap.Modal(document.getElementById('deleteCommentModal'));
    modal.show();
});

// invite users

document.addEventListener('DOMContentLoaded', function () {
    const emailInput = document.querySelector('#inviteEmails');
    const hiddenInput = document.querySelector('#usersHidden');
    const form = document.querySelector('#inviteForm');

    if (emailInput && form && hiddenInput) {
        const tagify = new Tagify(emailInput, {
            delimiters: ", ",
            pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, // простая email-валидация
            enforceWhitelist: false,
            dropdown: {
                enabled: 0
            }
        });

        form.addEventListener('submit', function () {
            const emails = tagify.value.map(tag => tag.value);
            hiddenInput.value = emails.join(',');
        });
    }
});

document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById('inviteModal');
    if (!modal) return;

    modal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;
        if (!button) return;

        var listId = button.getAttribute('data-list-id');

        document.getElementById('inviteListId').value = listId;
    });
});

// pagination

document.addEventListener("click", function (e) {
    if (e.target.matches("button[data-page]")) {
        const page = parseInt(e.target.dataset.page);
        const url = e.target.dataset.url;

        console.log("Paging button click");

        if (!url) return;

        const filterJson = document.getElementById(e.target.dataset.storage).value;
        const filter = JSON.parse(filterJson);
        filter.PageNumber = page;

        console.log(JSON.stringify(filter));

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(filter)
        })
            .then(response => {
                if (response.redirected) {
                    window.location.href = response.url;
                } else {
                    console.log("Response received, but no redirect");
                }
            })
            .catch(error => console.error("Fetch error:", error));
    }
});

// Task list header

document.addEventListener("DOMContentLoaded", function () {
    const header = document.getElementById("page-header");
    const main = document.getElementById("main-content");
    if (header && main) {
        const updatePadding = () => {
            const height = header.offsetHeight;
            main.style.paddingTop = height + "px";

            // Устанавливаем переменную
            document.documentElement.style.setProperty('--page-header-height', `${height}px`);
        };

        updatePadding();
        window.addEventListener("resize", updatePadding);
    }
});

// tag modal

document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById('tagModal');
    if (!modal) return;

    const input = document.querySelector('#taskTags');

    const tagify = new Tagify(input, {
        whitelist: [],
        dropdown: {
            maxItems: 20,
            classname: "task-tag",
            enabled: 0,
            closeOnSelect: false
        },
        originalInputValueFormat: valuesArr => valuesArr.map(item => item.value).join(',')
    });

    modal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;
        if (!button) return;

        var taskId = button.getAttribute('data-task-id');
        var listId = button.getAttribute('data-list-id');
        var tagsAttr = button.getAttribute('data-tags');

        document.getElementById('tagTaskId').value = taskId;
        document.getElementById('tagListId').value = listId;

        tagify.removeAllTags();

        const raw = button.getAttribute('data-available-tags');
        const availableTags = raw.split(',').map(t => t.trim()).filter(t => t.length > 0);
        tagify.settings.whitelist = availableTags;

        console.log(availableTags);

        const tags = tagsAttr.split(',').map(t => t.trim()).filter(t => t.length > 0);
        tagify.addTags(tags);
    });

    document.getElementById('tag-modal-form').addEventListener('submit', function (e) {
        console.log("Submit");

        if (input.value == null) {
            console.log("Input is NULL");
        }
        else {
            console.log("Input is: " + input.value);
        }

        if (!input.value) {
            input.value = ''; // ensure that field will not be NULL
        }
    });
});
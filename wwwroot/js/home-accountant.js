(function () {
    const config = window.accountantHomeConfig;
    const queueSection = document.getElementById('accountantQueueSection');
    const modalEl = document.getElementById('requestDetailsModal');

    if (!config || !queueSection || !modalEl) {
        return;
    }

    const customType = config.customType;
    const accountantId = config.accountantId;
    const typeLabels = config.typeLabels;
    const statusLabels = config.statusLabels;
    const statusBadgeClasses = config.statusBadgeClasses;
    const allowedTransitions = config.allowedTransitions;

    const detailLoading = document.getElementById('detailLoading');
    const detailError = document.getElementById('detailError');
    const detailContent = document.getElementById('detailContent');
    const detailRequestId = document.getElementById('detailRequestId');
    const detailEmployee = document.getElementById('detailEmployee');
    const detailType = document.getElementById('detailType');
    const detailCopies = document.getElementById('detailCopies');
    const detailReason = document.getElementById('detailReason');
    const detailStatus = document.getElementById('detailStatus');
    const statusTerminalNote = document.getElementById('statusTerminalNote');
    const statusChangeSection = document.getElementById('statusChangeSection');
    const statusChangeSuccess = document.getElementById('statusChangeSuccess');
    const statusChangeError = document.getElementById('statusChangeError');
    const newStatusSelect = document.getElementById('newStatusSelect');
    const saveStatusBtn = document.getElementById('saveStatusBtn');

    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    let currentRequestId = null;

    function formatType(details) {
        const name = typeLabels[details.type] || details.type;
        if (details.type === customType && details.customTypeName) {
            return name + ' («' + details.customTypeName + '»)';
        }
        return name;
    }

    function formatEmployee(details) {
        if (details.employeeDepartment) {
            return details.employeeFullName + ' (' + details.employeeDepartment + ')';
        }
        return details.employeeFullName;
    }

    function formatDate(iso) {
        const date = new Date(iso);
        return date.toLocaleString('ru-RU', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    function statusBadgeHtml(status) {
        const badge = statusBadgeClasses[status] || 'text-bg-secondary';
        const statusName = statusLabels[status] || status;
        return '<span class="badge ' + badge + '">' + statusName + '</span>';
    }

    function hideStatusMessages() {
        statusChangeSuccess.classList.add('d-none');
        statusChangeSuccess.textContent = '';
        statusChangeError.classList.add('d-none');
        statusChangeError.textContent = '';
    }

    function resetDetailView() {
        currentRequestId = null;
        detailLoading.classList.remove('d-none');
        detailError.classList.add('d-none');
        detailError.textContent = '';
        detailContent.classList.add('d-none');
        hideStatusMessages();
    }

    function showDetailError(message) {
        detailLoading.classList.add('d-none');
        detailContent.classList.add('d-none');
        detailError.textContent = message;
        detailError.classList.remove('d-none');
    }

    function updateStatusForm(currentStatus) {
        hideStatusMessages();
        const targets = allowedTransitions[currentStatus] || [];

        if (!targets.length) {
            statusChangeSection.classList.add('d-none');
            statusTerminalNote.classList.remove('d-none');
            return;
        }

        statusTerminalNote.classList.add('d-none');
        statusChangeSection.classList.remove('d-none');
        newStatusSelect.innerHTML = targets.map(function (status) {
            const label = statusLabels[status] || status;
            return '<option value="' + status + '">' + label + '</option>';
        }).join('');
    }

    function showDetails(details) {
        detailLoading.classList.add('d-none');
        detailError.classList.add('d-none');

        currentRequestId = details.id;
        detailRequestId.textContent = String(details.id);
        detailEmployee.textContent = formatEmployee(details);
        detailType.textContent = formatType(details);
        detailCopies.textContent = String(details.copiesCount);
        detailReason.textContent = details.reason;
        detailStatus.innerHTML = statusBadgeHtml(details.status);

        updateStatusForm(details.status);
        detailContent.classList.remove('d-none');
    }

    async function loadQueue() {
        const tbody = document.getElementById('queueTableBody');
        const emptyEl = document.getElementById('queueEmpty');
        const tableWrap = document.getElementById('queueTableWrap');
        if (!tbody || !emptyEl || !tableWrap) return;

        const res = await fetch('/api/requests');
        if (!res.ok) return;

        const items = await res.json();
        if (!items.length) {
            tbody.innerHTML = '';
            emptyEl.classList.remove('d-none');
            tableWrap.classList.add('d-none');
            return;
        }

        emptyEl.classList.add('d-none');
        tableWrap.classList.remove('d-none');
        tbody.innerHTML = items.map(function (item) {
            return '<tr class="js-request-row" data-request-id="' + item.id + '" role="button" style="cursor: pointer;">' +
                '<td>' + item.id + '</td>' +
                '<td>' + escapeHtml(item.employeeFullName) + '</td>' +
                '<td>' + escapeHtml(typeLabels[item.type] || item.type) + '</td>' +
                '<td>' + item.copiesCount + '</td>' +
                '<td>' + statusBadgeHtml(item.status) + '</td>' +
                '<td>' + formatDate(item.createdAtUtc) + '</td>' +
                '</tr>';
        }).join('');
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    async function openRequestDetails(requestId) {
        resetDetailView();
        modal.show();

        try {
            const res = await fetch('/api/requests/' + requestId);
            const payload = await res.json().catch(function () { return {}; });

            if (!res.ok) {
                throw new Error(payload.error || 'Не удалось загрузить заявку.');
            }

            showDetails(payload);
        } catch (err) {
            showDetailError(err.message || 'Произошла ошибка.');
        }
    }

    async function saveStatus() {
        if (!currentRequestId) return;

        hideStatusMessages();
        saveStatusBtn.disabled = true;

        try {
            const newStatus = Number(newStatusSelect.value);
            const url = '/api/requests/' + currentRequestId + '/status?accountantId=' + accountantId;

            const res = await fetch(url, {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ newStatus: newStatus })
            });

            const payload = await res.json().catch(function () { return {}; });

            if (!res.ok) {
                throw new Error(payload.error || 'Не удалось изменить статус.');
            }

            showDetails(payload);
            statusChangeSuccess.textContent = 'Статус успешно обновлён.';
            statusChangeSuccess.classList.remove('d-none');
            await loadQueue();
        } catch (err) {
            statusChangeError.textContent = err.message || 'Произошла ошибка.';
            statusChangeError.classList.remove('d-none');
        } finally {
            saveStatusBtn.disabled = false;
        }
    }

    queueSection.addEventListener('click', function (e) {
        const row = e.target.closest('.js-request-row');
        if (!row) return;

        const requestId = row.dataset.requestId;
        if (!requestId) return;

        openRequestDetails(requestId);
    });

    saveStatusBtn.addEventListener('click', saveStatus);
    modalEl.addEventListener('hidden.bs.modal', resetDetailView);
})();

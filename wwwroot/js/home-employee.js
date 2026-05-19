(function () {
    const config = window.employeeHomeConfig;
    const form = document.getElementById('createRequestForm');
    const requestsSection = document.getElementById('myRequestsSection');

    if (!config || !form || !requestsSection) {
        return;
    }

    const customType = config.customType;
    const employeeId = config.employeeId;
    const typeLabels = config.typeLabels;
    const statusLabels = config.statusLabels;
    const statusBadgeClasses = config.statusBadgeClasses;

    const typeSelect = document.getElementById('certificateType');
    const customBlock = document.getElementById('customTypeBlock');
    const duplicateWarning = document.getElementById('duplicateWarning');
    const formErrorAlert = document.getElementById('formErrorAlert');
    const submitBtn = document.getElementById('submitRequestBtn');
    const pageAlert = document.getElementById('pageAlert');
    const modalEl = document.getElementById('createRequestModal');
    const $form = $(form);

    let confirmDuplicate = false;

    function toggleCustom() {
        if (!typeSelect || !customBlock) return;
        customBlock.style.display = typeSelect.value === String(customType) ? 'block' : 'none';
    }

    function hideFormError() {
        formErrorAlert.classList.add('d-none');
        formErrorAlert.textContent = '';
    }

    function showFormError(message) {
        formErrorAlert.textContent = message;
        formErrorAlert.classList.remove('d-none');
    }

    function resetDuplicateFlow() {
        confirmDuplicate = false;
        duplicateWarning.classList.add('d-none');
        submitBtn.textContent = 'Отправить';
    }

    function resetForm() {
        form.reset();
        hideFormError();
        resetDuplicateFlow();
        $form.find('.text-danger').empty();
        $form.find('.input-validation-error').removeClass('input-validation-error');
        toggleCustom();
    }

    function showPageAlert(message, cssClass) {
        pageAlert.textContent = message;
        pageAlert.className = 'col-lg-8 mx-auto alert ' + cssClass;
        pageAlert.classList.remove('d-none');
    }

    function setSubmitting(isSubmitting) {
        submitBtn.disabled = isSubmitting;
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

    function renderRequests(items) {
        const tbody = document.getElementById('myRequestsBody');
        const emptyEl = document.getElementById('requestsEmpty');
        const tableWrap = document.getElementById('requestsTableWrap');
        if (!tbody || !emptyEl || !tableWrap) return;

        if (!items.length) {
            tbody.innerHTML = '';
            emptyEl.classList.remove('d-none');
            tableWrap.classList.add('d-none');
            return;
        }

        emptyEl.classList.add('d-none');
        tableWrap.classList.remove('d-none');
        tbody.innerHTML = items.map(function (item) {
            const status = item.status;
            const badge = statusBadgeClasses[status] || 'text-bg-secondary';
            const statusName = statusLabels[status] || status;
            const typeName = typeLabels[item.type] || item.type;
            return '<tr>' +
                '<td>' + item.id + '</td>' +
                '<td>' + typeName + '</td>' +
                '<td>' + item.copiesCount + '</td>' +
                '<td><span class="badge ' + badge + '">' + statusName + '</span></td>' +
                '<td>' + formatDate(item.createdAtUtc) + '</td>' +
                '</tr>';
        }).join('');
    }

    async function loadRequests() {
        const res = await fetch('/api/employees/' + employeeId + '/requests');
        if (!res.ok) return;
        const items = await res.json();
        renderRequests(items);
    }

    if (typeSelect) {
        typeSelect.addEventListener('change', function () {
            resetDuplicateFlow();
            toggleCustom();
        });
        toggleCustom();
    }

    if (modalEl) {
        modalEl.addEventListener('hidden.bs.modal', resetForm);
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        hideFormError();

        if (!$form.valid()) {
            return;
        }

        const type = Number(typeSelect.value);
        const customTypeName = document.getElementById('customTypeName')?.value?.trim() || null;
        const copiesCount = Number(document.getElementById('copiesCount').value);
        const reason = document.getElementById('reason').value.trim();

        if (type === customType && !customTypeName) {
            showFormError('Укажите название произвольной справки.');
            return;
        }

        setSubmitting(true);

        try {
            if (!confirmDuplicate) {
                const similarUrl = '/api/employees/' + employeeId + '/requests/similar?type=' + type;
                const similarRes = await fetch(similarUrl);
                if (!similarRes.ok) {
                    const err = await similarRes.json().catch(function () { return {}; });
                    throw new Error(err.error || 'Не удалось проверить дубликаты.');
                }
                const similar = await similarRes.json();
                if (similar.hasSimilarActive) {
                    confirmDuplicate = true;
                    duplicateWarning.classList.remove('d-none');
                    submitBtn.textContent = 'Всё равно отправить';
                    return;
                }
            }

            const createRes = await fetch('/api/requests', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    employeeId: employeeId,
                    type: type,
                    copiesCount: copiesCount,
                    reason: reason,
                    customTypeName: customTypeName
                })
            });

            const payload = await createRes.json().catch(function () { return {}; });

            if (!createRes.ok) {
                throw new Error(payload.error || 'Не удалось отправить заявку.');
            }

            let message = 'Заявка №' + payload.request.id + ' успешно отправлена.';
            if (payload.similarActiveWarning?.hasSimilarActive) {
                message += ' Обратите внимание: у вас уже была активная заявка того же типа.';
            }

            bootstrap.Modal.getOrCreateInstance(modalEl).hide();
            resetForm();
            showPageAlert(message, 'alert-success');
            await loadRequests();
        } catch (err) {
            showFormError(err.message || 'Произошла ошибка.');
        } finally {
            setSubmitting(false);
        }
    });
})();

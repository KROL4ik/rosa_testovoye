(function (global) {
    function formatTypeLabel(item, typeLabels, customType) {
        const name = typeLabels[item.type] || item.type;
        if (item.type === customType && item.customTypeName) {
            return name + ' («' + item.customTypeName + '»)';
        }
        return name;
    }

    function groupKey(item, customType) {
        const type = Number(item.type);
        const employeeId = item.employeeId;
        if (type === customType) {
            const name = (item.customTypeName || '').trim().toLowerCase();
            return employeeId + ':' + type + ':' + name;
        }
        return employeeId + ':' + type;
    }

    function employeeTypeKey(item, customType) {
        const type = Number(item.type);
        if (type === customType) {
            const name = (item.customTypeName || '').trim().toLowerCase();
            return type + ':' + name;
        }
        return String(type);
    }

    function buildGroups(items, keyFn) {
        const map = new Map();
        items.forEach(function (item) {
            const key = keyFn(item);
            if (!map.has(key)) {
                map.set(key, []);
            }
            map.get(key).push(item);
        });

        const groups = Array.from(map.values());
        groups.forEach(function (group) {
            group.sort(function (a, b) {
                return new Date(b.createdAtUtc) - new Date(a.createdAtUtc);
            });
        });

        groups.sort(function (a, b) {
            return new Date(b[0].createdAtUtc) - new Date(a[0].createdAtUtc);
        });

        return groups;
    }

    global.RequestGrouping = {
        formatTypeLabel: formatTypeLabel,
        groupKey: groupKey,
        employeeTypeKey: employeeTypeKey,
        buildGroups: buildGroups
    };
})(window);

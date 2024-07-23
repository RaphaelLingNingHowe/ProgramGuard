async function fetchData(url, method, data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
    };

    if (data) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(url, options);
        const status = response.status;
        const responseData = status === 204 ? {} : await response.json();
        return { status, data: responseData };
    } catch (error) {
        console.error('API call error:', error);
        throw error;
    }
}

function createCachedFunction(fn) {
    let cachedResult = null;
    let isFetching = false;
    let waitingPromises = [];

    return async function (...args) {
        // 如果已經有緩存結果，直接返回
        if (cachedResult !== null) {
            return cachedResult;
        }

        // 如果正在獲取數據，等待結果
        if (isFetching) {
            return new Promise((resolve) => waitingPromises.push(resolve));
        }

        // 開始獲取數據
        isFetching = true;
        try {
            const result = await fn(...args);
            cachedResult = result;

            // 解決所有等待的 promise
            waitingPromises.forEach(resolve => resolve(result));
            waitingPromises = [];

            return result;
        } catch (error) {
            // 清除等待的 promise
            waitingPromises = [];
            throw error;
        } finally {
            isFetching = false;
        }
    }
}
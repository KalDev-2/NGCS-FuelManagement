class EthiopianDateConverter {
    static JdOffset = 1723856;

    static toEthiopianDate(dateTime) {
        let jdn = this.toJDN(dateTime);
        return this.toEthiopianDateFromJDN(jdn);
    }

    static toJDN(dateTime) {
        let num = Math.floor((14 - dateTime.getMonth()) / 12);
        let num2 = dateTime.getFullYear() + 4800 - num;
        let num3 = dateTime.getMonth() + 12 * num - 3;
        return dateTime.getDate() + Math.floor((153 * num3 + 2) / 5) + 365 * num2 + Math.floor(num2 / 4) - Math.floor(num2 / 100) + Math.floor(num2 / 400) - 32045;
    }

    static toEthiopianDateFromJDN(jdn) {
        let num = (jdn - this.JdOffset) % 1461;
        let num2 = num % 365 + 365 * Math.floor(num / 1460);
        let year = 4 * Math.floor((jdn - this.JdOffset) / 1461) + Math.floor(num / 365) - Math.floor(num / 1460);
        let month = Math.floor(num2 / 30) + 1;
        let day = num2 % 30 + 1;
        return new Date(year, month, day);
    }

    static toGregorianDate(year, month, day) {
        this.validate(year, month, day);
        let jdn = this.fromEthiopianDateToJDN(year, month, day);
        return this.toGregorianDateFromJDN(jdn);
    }

    static toGregorianDateFromJDN(jdn) {
        let num = jdn + 68569;
        let num2 = Math.floor(4 * num / 146097);
        num -= Math.floor((146097 * num2 + 3) / 4);
        let num3 = Math.floor(4000 * (num + 1) / 1461001);
        num = num - Math.floor(1461 * num3 / 4) + 31;
        let num4 = Math.floor(80 * num / 2447);
        let day = num - Math.floor(2447 * num4 / 80);
        num = Math.floor(num4 / 11);
        num4 = num4 + 2 - 12 * num;
        num3 = 100 * (num2 - 49) + num3 + num;
        return new Date(num3, num4, day);
    }

    static validate(year, month, day) {
        if (month < 1 || month > 13 || (month === 13 && year % 4 === 3 && day > 6) || (month === 13 && year % 4 !== 3 && day > 5) || day < 1 || day > 30) {
            throw new Error("Year, Month, and Day parameters describe an un-representable EthiopianDateTime.");
        }
    }

    static fromEthiopianDateToJDN(year, month, day) {
        return 1724221 + 365 * (year - 1) + Math.floor(year / 4) + 30 * month + day - 31;
    }
}

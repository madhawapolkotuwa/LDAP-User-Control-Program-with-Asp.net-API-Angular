export class Member{
    uid!:string;
    cn!:string;
    sn!:string;
    employeeNumber!:string;
    employeeType!:string;
    createdTimestamp!:string;
}

export class NewMember{
    email!:string;
    password!:string;
    surename!:string;
    commonname!:string;
    employeenumber!:Number;
    employeetype!:string;
}

export class ModifiedMember{
    email!:string;
    password!:string;
    surename!:string;
    commonname!:string;
    employeenumber!:Number;
    employeetype!:string;
    modifiedCommonname: string = '';
    modifiedSurename: string = '';
    modifiedEmployeetype: string = '';
    modifiedEmployeenumber: number = 0;
}

export class DeleteMember{
    email!:string;
    employeetype!:string;
}
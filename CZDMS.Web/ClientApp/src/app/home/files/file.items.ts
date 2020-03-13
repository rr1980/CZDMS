
export class PathInfo{
    key: string;
    name: string;
}

export class FileItem {
    name: string;
    pathInfo?: PathInfo[];
    parentPath: string;
    isDirectory: boolean;
    size?: number;
    items?: FileItem[];
}


export const fileItems: FileItem[] = [
    {
        'name': 'Documents',
        'parentPath': 'root',
        'isDirectory': true,
        'items': [
            {
                'name': 'Projects',
                'parentPath': 'Documents',
                'isDirectory': true,
                'items': [
                    {
                        'name': 'About.rtf',
                        'parentPath': 'Projects',
                        'isDirectory': false,
                        'size': 1024
                    },
                    {
                        'name': 'Passwords.rtf',
                        'parentPath': 'Projects',
                        'isDirectory': false,
                        'size': 2048
                    }
                ]
            },
            {
                'name': 'About.xml',
                'parentPath': 'Documents',
                'isDirectory': false,
                'size': 1024
            },
            {
                'name': 'Managers.rtf',
                'parentPath': 'Documents',
                'isDirectory': false,
                'size': 2048
            },
            {
                'name': 'ToDo.txt',
                'parentPath': 'Documents',
                'isDirectory': false,
                'size': 3072
            }
        ],
    },
    {
        'name': 'Images',
        'parentPath': 'root',
        'isDirectory': true,
        'items': [
            {
                'name': 'logo.png',
                'parentPath': 'Images',
                'isDirectory': false,
                'size': 20480
            },
            {
                'name': 'banner.gif',
                'parentPath': 'Images',
                'isDirectory': false,
                'size': 10240
            }
        ]
    },
    {
        'name': 'System',
        'parentPath': 'root',
        'isDirectory': true,
        'items': [
            {
                'name': 'Employees.txt',
                'parentPath': 'System',
                'isDirectory': false,
                'size': 3072
            },
            {
                'name': 'PasswordList.txt',
                'parentPath': 'System',
                'isDirectory': false,
                'size': 5120
            }
        ]
    },
    {
        'name': 'Description.rtf',
        'parentPath': 'root',
        'isDirectory': false,
        'size': 1024
    },
    {
        'name': 'Description.txt',
        'parentPath': 'root',
        'isDirectory': false,
        'size': 2048
    }
];
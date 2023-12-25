// App.js
import React from 'react';
import TabbedTables from './TabbedTables';

const tabs = [
  { label: 'EduDocs', apiUrl: 'http://localhost:5260/api/EduDocs', columns: ['Inila', 'RegEduDoc', 'SndEduDoc', 'DateEnd'] },
  { label: 'HfInfos', apiUrl: 'http://localhost:5260/api/HfInfos', columns: ['IdInfo', 'Inila', 'RegOrgNum', 'Status', 'DateOrd'] },
  { label: 'Institutes', apiUrl: 'http://localhost:5260/api/Instituts', columns: ['RegEduDoc', 'Type', 'Name'] },
  { label: 'PersonalData', apiUrl: 'http://localhost:5260/api/PersonalDatas', columns: ['Inila', 'Fcs', 'Itn', 'Address', 'SnPassport', 'Married', 'Kids'] },
  { label: 'Works', apiUrl: 'http://localhost:5260/api/Works', columns: ['RegOrgNum', 'NameOrg', 'ItnOrg', 'OrgAddress'] },
];

function App() {
  return (
    <div className="App">
      <TabbedTables tabs={tabs} />
    </div>
  );
}

export default App;

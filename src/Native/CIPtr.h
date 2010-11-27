template<class I>
class __declspec(dllexport) CIPtr
{
	I* m_pI;

public:
	virtual ~CIPtr() { Release(); }

	CIPtr() { m_pI = NULL; }
	CIPtr( I* p ) { m_pI = p; if( m_pI ) m_pI->AddRef(); }
	CIPtr( const CIPtr<I>& p ) { m_pI = p.m_pI; if( m_pI ) m_pI->AddRef(); }

	operator I*() { return m_pI; }
	I& operator*() { return *m_pI; }
	I** operator&() { return &m_pI; }
	I* operator->() { return m_pI; }
	I* operator=( I* p ) { if( m_pI ) m_pI->Release(); m_pI = p; if( m_pI ) m_pI->AddRef(); return m_pI; }
	I* operator=( const CIPtr<I>& p )
	{
		if( m_pI )
			m_pI->Release();

		m_pI = p.m_pI;
		if( m_pI )
			m_pI->AddRef();
		return m_pI;
	}

	bool operator!() const { return (m_pI == NULL); }
	bool operator==( const I* p ) const { return (m_pI == p); }
	bool operator==( const CIPtr<I>& p ) const { return (m_pI == p.m_pI); }

public:
	unsigned long Release()
	{
		unsigned long ref = 0;
		if (m_pI)
			ref = m_pI->Release();
		m_pI = NULL;
		return ref;
	}
};
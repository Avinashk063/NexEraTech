document.addEventListener('DOMContentLoaded', () => {

    // --- Dynamic Header Styling ---
    const header = document.querySelector('header');

    function handleHeaderScroll() {
        if (!header) return;
        if (window.scrollY > 50) header.classList.add('scrolled');
        else header.classList.remove('scrolled');
    }

    // (carousel removed - keeping page behavior simple)

    handleHeaderScroll();
    window.addEventListener('scroll', handleHeaderScroll);

    // --- Smooth Scrolling for Navigation ---
    const navLinks = document.querySelectorAll('header nav a[href^="#"], .scroll-down-btn');

    navLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            const targetId = this.getAttribute('href');
            if (!targetId || !targetId.startsWith('#') || targetId === '#') return;
            e.preventDefault();
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                const headerHeight = header ? header.offsetHeight : 0;
                const offsetPosition = targetElement.getBoundingClientRect().top + window.scrollY - headerHeight;
                window.scrollTo({ top: offsetPosition, behavior: 'smooth' });
            }
        });
    });

    // --- Active Nav Link Highlighting ---
    const sectionLinks = Array.from(document.querySelectorAll('header nav a[href^="#"]'));
    const sections = sectionLinks
        .map(link => document.querySelector(link.getAttribute('href')))
        .filter(Boolean);

    function updateActiveNavLink() {
        if (!sectionLinks.length || !sections.length) return;

        const headerHeight = header ? header.offsetHeight : 0;
        const checkpoint = window.scrollY + headerHeight + 120;

        let activeSectionId = sections[0].id;
        sections.forEach(section => {
            if (section.offsetTop <= checkpoint) {
                activeSectionId = section.id;
            }
        });

        sectionLinks.forEach(link => {
            const isActive = link.getAttribute('href') === `#${activeSectionId}`;
            link.classList.toggle('active', isActive);
        });
    }

    updateActiveNavLink();
    window.addEventListener('scroll', updateActiveNavLink);

    // --- Mobile nav toggle ---
    // Mobile nav toggle (use id for nav for reliability)
    const mobileToggle = document.querySelector('.mobile-toggle');
    const nav = document.getElementById('main-nav') || document.querySelector('header nav');
    const backdrop = document.querySelector('.mobile-backdrop');

    if (mobileToggle && nav) {
        // ensure toggle is clickable and announce state
        mobileToggle.setAttribute('aria-expanded', 'false');

        const toggleNav = (ev) => {
            if (ev) ev.stopPropagation(); // prevent document-level click from immediately closing
            // defensive: ensure nav exists
            if (!nav) return;
            const isOpen = nav.classList.toggle('open');
            mobileToggle.classList.toggle('open', isOpen);
            document.body.classList.toggle('nav-open', isOpen);
            mobileToggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
            // show/hide backdrop
            if (backdrop) {
                backdrop.style.display = isOpen ? 'block' : 'none';
                // small delay to allow CSS transition if needed
                requestAnimationFrame(() => {
                    backdrop.style.pointerEvents = isOpen ? 'auto' : 'none';
                    backdrop.style.opacity = isOpen ? '1' : '0';
                });
            }
        };

        // attach click handler for toggle (click is reliable across devices)
        mobileToggle.addEventListener('click', toggleNav);

        // Prevent clicks inside the nav from bubbling to document (which closes the menu)
        nav.addEventListener('click', (ev) => ev.stopPropagation());
    }

    // close mobile nav when a nav link is clicked (so CTA remains visible and we return to page content)
    if (nav) {
        const mobileNavLinks = nav.querySelectorAll('a[href^="#"]');
        mobileNavLinks.forEach(l => l.addEventListener('click', () => {
            if (nav.classList.contains('open')) {
                nav.classList.remove('open');
                if (mobileToggle) mobileToggle.classList.remove('open');
                document.body.classList.remove('nav-open');
                if (backdrop) { backdrop.style.pointerEvents = 'none'; backdrop.style.opacity = '0'; }
            }
        }));
    }

    // close when clicking outside the nav on mobile
    document.addEventListener('click', (ev) => {
        if (!nav || !mobileToggle) return;
        if (!nav.classList.contains('open')) return;
        const target = ev.target;
        const backdrop = document.querySelector('.mobile-backdrop');
        if (nav.contains(target) || mobileToggle.contains(target)) return;
        if (backdrop && backdrop.contains(target)) {
            // clicked on backdrop: close
            nav.classList.remove('open');
            mobileToggle.classList.remove('open');
            document.body.classList.remove('nav-open');
            mobileToggle.setAttribute('aria-expanded', 'false');
            return;
        }
        nav.classList.remove('open');
        mobileToggle.classList.remove('open');
        document.body.classList.remove('nav-open');
    });

    // close on Escape
    document.addEventListener('keydown', (ev) => {
        if (ev.key === 'Escape' && nav && nav.classList.contains('open')) {
            nav.classList.remove('open');
            mobileToggle.classList.remove('open');
            document.body.classList.remove('nav-open');
        }
    });

    // --- Subtle Scroll Reveal ---
    const revealTargets = document.querySelectorAll('.hero-text, .hero-panel, .project-card, .skill-group-card, .about-visual, .about-text, .contact-card, .footer-inner');
    const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    if (revealTargets.length) {
        revealTargets.forEach((el, index) => {
            el.classList.add('reveal');
            el.style.transitionDelay = `${Math.min(index * 55, 240)}ms`;
        });

        if (reducedMotion) {
            revealTargets.forEach(el => el.classList.add('is-visible'));
        } else {
            const observer = new IntersectionObserver((entries, obs) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('is-visible');
                        obs.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.15, rootMargin: '0px 0px -40px 0px' });

            revealTargets.forEach(el => observer.observe(el));
        }
    }

    // newsletter form (placeholder)
    const newsletterForm = document.querySelector('.newsletter-form');
    if (newsletterForm) {
        newsletterForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const email = newsletterForm.querySelector('input[type="email"]').value;
            alert('Thanks! Subscribed: ' + email);
            newsletterForm.reset();
        });
    }

});

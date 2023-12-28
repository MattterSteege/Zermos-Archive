﻿const maskStyle = (maskColor) => `
        position: absolute;
        border-radius: 4px;
        box-shadow: 0 0 0 9999px ${maskColor};
        z-index: 10001 !important;
        transition: all .3s;
    `;

const slotStyle = () => `
        position: absolute;
        z-index: 10002 !important;
        transition: all .3s;
    `;

const layerStyle = () => `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        z-index: 10000 !important;
    `;

const defaultOptions = {
    prefix: 'zermostour',
    padding: 5,
    maskColor: 'rgba(0, 0, 0, .5)',
    animate: true,
    slotPosition: 'top'
};

class ZermosTour {
    constructor(options = {}) {
        this.options = {
            ...defaultOptions,
            layerEvent: this.over.bind(this),
            ...options
        };

        this.mask = null;
        this.slot = null;
        this.layer = null;
    }

    _createMask() {
        if (!this.mask) {
            this.mask = document.createElement('div');
            this.mask.setAttribute('class', this.options.prefix + '-mask');
            this.mask.setAttribute('style', maskStyle(this.options.maskColor));
            document.body.appendChild(this.mask);
        }
    }

    _createSlot(html) {
        if (!this.slot) {
            this.slot = document.createElement('div');
            this.slot.setAttribute('style', slotStyle());
            document.body.appendChild(this.slot);
        }
        this.slot.setAttribute('class', `${this.options.prefix}-slot ${this.options.prefix}-slot_${this.options.slotPosition}`);
        this.slot.innerHTML = html;
    }

    _createLayer() {
        if (!this.layer) {
            this.layer = document.createElement('div');
            this.layer.setAttribute('class', this.options.prefix + '-layer');
            this.layer.setAttribute('style', layerStyle());
            this.layer.addEventListener('click', this.options.layerEvent);
            document.addEventListener('keydown', (e) => {
                if (e.code === 'Space' || e.code === 'ArrowRight' || e.code === 'ArrowDown' || e.code === 'Enter') {
                    this.options.layerEvent();
                }
            });
            document.body.appendChild(this.layer);
        }
    }

    _setPosition(el, attrs) {
        ['top', 'left', 'width', 'height'].forEach((attr, index) => {
            if (attrs[index]) {
                if (attr === 'top' || attr === 'left') {
                    const scrollDirection = `scroll${attr.charAt(0).toUpperCase() + attr.slice(1)}`;
                    let scrollDistance = 0;
                    if (document.documentElement && document.documentElement[scrollDirection]) {
                        scrollDistance = document.documentElement[scrollDirection];
                    } else {
                        scrollDistance = document.body[scrollDirection];
                    }
                    el.style[attr] = attrs[index] + scrollDistance + 'px';
                } else {
                    el.style[attr] = attrs[index] + 'px';
                }
            }
        });
    }

    _setSlotPosition(slot, slotPosition, maskTop, maskLeft, maskWidth, maskHeight)
    {        
        if (slotPosition === 'left') {
            const [x, y] = [maskLeft - slot.clientWidth, maskTop];
            slot.style.top = Math.max(Number(y), 5) + 'px';
            slot.style.left = (x - 10) + 'px';
        }
        else if (slotPosition === 'right') {
            const [x, y] = [maskLeft + maskWidth, maskTop];
            slot.style.top = Math.max(Number(y), 5) + 'px';
            slot.style.left = (x + 10) + 'px';
        }
        else if (slotPosition === 'bottom') {
            const [x, y] = [(maskLeft + maskWidth / 2), maskTop + maskHeight];
            slot.style.top = (y + 10) + 'px';
            slot.style.left = (x - slot.clientWidth / 2) + 'px';
        }
        else if (slotPosition === 'top') {
            const [x, y] = [(maskLeft + maskWidth / 2), maskTop - slot.clientHeight];
            slot.style.top = (y - 10) + 'px';
            slot.style.left = (x - slot.clientWidth / 2) + 'px';
        }
    }

    _show(targetSelector, slotHtml = '', keyNodes = []) {
        this._createMask();
        this._createSlot(slotHtml);
        this._createLayer();

        if (!this.options.animate) {
            this.mask.style.transition = null;
            this.slot.style.transition = null;
        }

        const target = document.querySelector(targetSelector);
        const {
            top,
            left,
            width,
            height
        } = target.getBoundingClientRect();
        const [maskTop, maskLeft, maskWidth, maskHeight] = [top - this.options.padding, left - this.options.padding, width + 2 * this.options.padding, height + 2 * this.options.padding];

        this._setPosition(this.mask, [maskTop, maskLeft, maskWidth, maskHeight]);

        const {
            width: slotWidth,
            height: slotHeight
        } = this.slot.getBoundingClientRect();
        
        const {
            slotPosition
        } = this.options;
        
        this._setSlotPosition(this.slot, slotPosition, maskTop, maskLeft, maskWidth, maskHeight);

        if (!slotHtml) {
            document.body.removeChild(this.slot);
            this.slot = null;
        }

        if (keyNodes.length) {
            keyNodes.forEach(({
                                  el,
                                  event
                              }) => {
                document.querySelector(el).addEventListener('click', event);
            });
        }
    }

    focus({
              el = '',
              slot = '',
              keyNodes = [],
              options = {}
          }) {
        if (Object.keys(options).length) {
            this.options = {
                ...this.options,
                ...options
            };
        }
        this._show(el, slot, keyNodes);
    }

    queue(tourList) {
        this.tourList = tourList;
        this.tourListLength = tourList.length;
        this.tourIndex = -1;

        return this
    }
    
    addToQueue(tourList) {
        this.tourList.push(tourList);
        this.tourListLength += tourList.length;
        this.tourIndex = -1;

        return this
    }

    run(isNext = true) {
        if (this.tourListLength && this.tourIndex < this.tourListLength - 1) {
            isNext ? this.tourIndex++ : this.tourIndex--;
            const tour = this.tourList[this.tourIndex];
            if (tour.options) {
                this.options = {
                    ...this.options,
                    ...tour.options
                };
            }
            this._show(tour.el, tour.slot, tour.keyNodes);
        } else {
            this.over();
        }
    }

    next() {
        this.run(true);
    }

    prev() {
        this.run(false);
    }

    over() {
        this.mask && document.body.removeChild(this.mask);
        this.slot && document.body.removeChild(this.slot);
        this.layer && document.body.removeChild(this.layer);

        ['mask', 'slot', 'layer'].forEach(attr => {
            this[attr] = null;
        });
    }
}